using System;
using UnityEngine;

namespace UtilityPackage.Attributes
{
	/// <summary>
	///     <para>Prevent this field from being edited in the inspector.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}