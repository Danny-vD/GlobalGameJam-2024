﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;

namespace Ink.Parsed
{
    public abstract class Expression : Object
    {
        private Container _prototypeRuntimeConstantExpression;
        public bool outputWhenComplete { get; set; }

        public override Runtime.Object GenerateRuntimeObject()
        {
            var container = new Container();

            // Tell Runtime to start evaluating the following content as an expression
            container.AddContent(ControlCommand.EvalStart());

            GenerateIntoContainer(container);

            // Tell Runtime to output the result of the expression evaluation to the output stream
            if (outputWhenComplete) container.AddContent(ControlCommand.EvalOutput());

            // Tell Runtime to stop evaluating the content as an expression
            container.AddContent(ControlCommand.EvalEnd());

            return container;
        }

        // When generating the value of a constant expression,
        // we can't just keep generating the same constant expression into
        // different places where the constant value is referenced, since then
        // the same runtime objects would be used in multiple places, which
        // is impossible since each runtime object should have one parent.
        // Instead, we generate a prototype of the runtime object(s), then
        // copy them each time they're used.
        public void GenerateConstantIntoContainer(Container container)
        {
            if (_prototypeRuntimeConstantExpression == null)
            {
                _prototypeRuntimeConstantExpression = new Container();
                GenerateIntoContainer(_prototypeRuntimeConstantExpression);
            }

            foreach (var runtimeObj in _prototypeRuntimeConstantExpression.content)
                container.AddContent(runtimeObj.Copy());
        }

        public abstract void GenerateIntoContainer(Container container);
    }

    public class BinaryExpression : Expression
    {
        public Expression leftExpression;
        public string opName;
        public Expression rightExpression;

        public BinaryExpression(Expression left, Expression right, string opName)
        {
            leftExpression = AddContent(left);
            rightExpression = AddContent(right);
            this.opName = opName;
        }

        public override void GenerateIntoContainer(Container container)
        {
            leftExpression.GenerateIntoContainer(container);
            rightExpression.GenerateIntoContainer(container);

            opName = NativeNameForOp(opName);

            container.AddContent(NativeFunctionCall.CallWithName(opName));
        }

        public override void ResolveReferences(Story context)
        {
            base.ResolveReferences(context);

            // Check for the following case:
            //
            //    (not A) ? B
            //
            // Since this easy to accidentally do:
            //
            //    not A ? B
            //
            // when you intend:
            //
            //    not (A ? B)
            if (NativeNameForOp(opName) == "?")
            {
                var leftUnary = leftExpression as UnaryExpression;
                if (leftUnary != null && (leftUnary.op == "not" || leftUnary.op == "!"))
                    Error("Using 'not' or '!' here negates '" + leftUnary.innerExpression +
                          "' rather than the result of the '?' or 'has' operator. You need to add parentheses around the (A ? B) expression.");
            }
        }

        private string NativeNameForOp(string opName)
        {
            if (opName == "and")
                return "&&";

            if (opName == "or")
                return "||";

            if (opName == "mod")
                return "%";

            if (opName == "has")
                return "?";

            if (opName == "hasnt")
                return "!?";

            return opName;
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", leftExpression, opName, rightExpression);
        }
    }

    public class UnaryExpression : Expression
    {
        public Expression innerExpression;
        public string op;

        public UnaryExpression(Expression inner, string op)
        {
            innerExpression = AddContent(inner);
            this.op = op;
        }

        private string nativeNameForOp
        {
            get
            {
                // Replace "-" with "_" to make it unique (compared to subtraction)
                if (op == "-")
                    return "_";
                if (op == "not")
                    return "!";
                return op;
            }
        }

        // Attempt to flatten inner expression immediately
        // e.g. convert (-(5)) into (-5)
        public static Expression WithInner(Expression inner, string op)
        {
            var innerNumber = inner as Number;
            if (innerNumber)
            {
                if (op == "-")
                {
                    if (innerNumber.value is int)
                        return new Number(-(int)innerNumber.value);
                    if (innerNumber.value is float) return new Number(-(float)innerNumber.value);
                }

                else if (op == "!" || op == "not")
                {
                    if (innerNumber.value is int)
                        return new Number((int)innerNumber.value == 0);
                    if (innerNumber.value is float)
                        return new Number((float)innerNumber.value == 0.0f);
                    if (innerNumber.value is bool) return new Number(!(bool)innerNumber.value);
                }

                throw new Exception("Unexpected operation or number type");
            }

            // Normal fallback
            var unary = new UnaryExpression(inner, op);
            return unary;
        }

        public override void GenerateIntoContainer(Container container)
        {
            innerExpression.GenerateIntoContainer(container);

            container.AddContent(NativeFunctionCall.CallWithName(nativeNameForOp));
        }

        public override string ToString()
        {
            return nativeNameForOp + innerExpression;
        }
    }

    public class IncDecExpression : Expression
    {
        private Runtime.VariableAssignment _runtimeAssignment;
        public Expression expression;
        public bool isInc;
        public Identifier varIdentifier;

        public IncDecExpression(Identifier varIdentifier, bool isInc)
        {
            this.varIdentifier = varIdentifier;
            this.isInc = isInc;
        }

        public IncDecExpression(Identifier varIdentifier, Expression expression, bool isInc) : this(varIdentifier,
            isInc)
        {
            this.expression = expression;
            AddContent(expression);
        }

        private string incrementDecrementWord
        {
            get
            {
                if (isInc)
                    return "increment";
                return "decrement";
            }
        }

        public override void GenerateIntoContainer(Container container)
        {
            // x = x + y
            // ^^^ ^ ^ ^
            //  4  1 3 2
            // Reverse polish notation: (x 1 +) (assign to x)

            // 1.
            container.AddContent(new Runtime.VariableReference(varIdentifier?.name));

            // 2.
            // - Expression used in the form ~ x += y
            // - Simple version: ~ x++
            if (expression)
                expression.GenerateIntoContainer(container);
            else
                container.AddContent(new IntValue(1));

            // 3.
            container.AddContent(NativeFunctionCall.CallWithName(isInc ? "+" : "-"));

            // 4.
            _runtimeAssignment = new Runtime.VariableAssignment(varIdentifier?.name, false);
            container.AddContent(_runtimeAssignment);
        }

        public override void ResolveReferences(Story context)
        {
            base.ResolveReferences(context);

            var varResolveResult = context.ResolveVariableWithName(varIdentifier?.name, this);
            if (!varResolveResult.found)
                Error("variable for " + incrementDecrementWord + " could not be found: '" + varIdentifier +
                      "' after searching: " + descriptionOfScope);

            _runtimeAssignment.isGlobal = varResolveResult.isGlobal;

            if (!(parent is Weave) && !(parent is FlowBase) && !(parent is ContentList))
                Error("Can't use " + incrementDecrementWord + " as sub-expression");
        }

        public override string ToString()
        {
            if (expression)
                return varIdentifier + (isInc ? " += " : " -= ") + expression;
            return varIdentifier + (isInc ? "++" : "--");
        }
    }

    public class MultipleConditionExpression : Expression
    {
        public MultipleConditionExpression(List<Expression> conditionExpressions)
        {
            AddContent(conditionExpressions);
        }

        public List<Expression> subExpressions => content.Cast<Expression>().ToList();

        public override void GenerateIntoContainer(Container container)
        {
            //    A && B && C && D
            // => (((A B &&) C &&) D &&) etc
            var isFirst = true;
            foreach (var conditionExpr in subExpressions)
            {
                conditionExpr.GenerateIntoContainer(container);

                if (!isFirst) container.AddContent(NativeFunctionCall.CallWithName("&&"));

                isFirst = false;
            }
        }
    }
}