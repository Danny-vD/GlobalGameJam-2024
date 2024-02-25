using System.Collections.Generic;
using Ink.Parsed;

namespace Ink
{
    public partial class InkParser
    {
        protected Object StartTag()
        {
            Whitespace();

            if (ParseString("#") == null)
                return null;

            if (parsingStringExpression)
                Error("Tags aren't allowed inside of strings. Please use \\# if you want a hash symbol.");
            // but allow us to continue anyway...
            var result = (Object)null;

            // End previously active tag before starting new one
            if (tagActive)
            {
                var contentList = new ContentList();
                contentList.AddContent(new Tag { isStart = false });
                contentList.AddContent(new Tag { isStart = true });
                result = contentList;
            }

            // Otherwise, just start a tag, no need for a content list
            else
            {
                result = new Tag { isStart = true };
            }

            tagActive = true;

            Whitespace();

            return result;
        }

        protected void EndTagIfNecessary(List<Object> outputContentList)
        {
            if (tagActive)
            {
                if (outputContentList != null)
                    outputContentList.Add(new Tag { isStart = false });
                tagActive = false;
            }
        }

        protected void EndTagIfNecessary(ContentList outputContentList)
        {
            if (tagActive)
            {
                if (outputContentList != null)
                    outputContentList.AddContent(new Tag { isStart = false });
                tagActive = false;
            }
        }
    }
}