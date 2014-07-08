﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static partial class GeneratedNames
    {
        internal const string SynthesizedLocalNamePrefix = "CS$";

        internal static bool IsGeneratedName(string memberName)
        {
            return memberName.Length > 0 && memberName[0] == '<';
        }

        internal static string MakeBackingFieldName(string propertyName)
        {
            return "<" + propertyName + ">k__BackingField";
        }

        internal static string MakeLambdaMethodName(string containingMethodName, int uniqueId)
        {
            return "<" + containingMethodName + ">b__" + uniqueId;
        }

        internal static string MakeAnonymousDisplayClassName(int uniqueId)
        {
            return "<>c__DisplayClass" + uniqueId;
        }

        internal static string MakeAnonymousTypeTemplateName(int index, int submissionSlotIndex, string moduleId)
        {
            var name = "<" + moduleId + ">f__AnonymousType" + index;
            if (submissionSlotIndex >= 0)
            {
                name += "#" + submissionSlotIndex;
            }
            return name;
        }

        internal const string AnonymousNamePrefix = "<>f__AnonymousType";

        internal static bool TryParseAnonymousTypeTemplateName(string name, out int index)
        {
            // No callers require anonymous types from net modules,
            // so names with module id are ignored.
            if (name.StartsWith(AnonymousNamePrefix, StringComparison.Ordinal))
            {
                if (int.TryParse(name.Substring(AnonymousNamePrefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out index))
                {
                    return true;
                }
            }

            index = -1;
            return false;
        }

        internal static string MakeAnonymousTypeBackingFieldName(string propertyName)
        {
            return "<" + propertyName + ">i__Field";
        }

        internal static string MakeAnonymousTypeParameterName(string propertyName)
        {
            return "<" + propertyName + ">j__TPar";
        }

        internal static bool TryParseAnonymousTypeParameterName(string typeParameterName, out string propertyName)
        {
            if (typeParameterName.StartsWith("<", StringComparison.Ordinal) &&
                typeParameterName.EndsWith(">j__TPar", StringComparison.Ordinal))
            {
                propertyName = typeParameterName.Substring(1, typeParameterName.Length - 9);
                return true;
            }

            propertyName = null;
            return false;
        }

        internal static string MakeIteratorOrAsyncDisplayClassName(string methodName, int uniqueId)
        {
            methodName = EnsureNoDotsInTypeName(methodName);
            return "<" + methodName + ">d__" + uniqueId;
        }

        private static string EnsureNoDotsInTypeName(string name)
        {
            // CLR generally allows names with dots, however some APIs like IMetaDataImport
            // can only return full type names combined with namespaces. 
            // see: http://msdn.microsoft.com/en-us/library/ms230143.aspx (IMetaDataImport::GetTypeDefProps)
            // When working with such APIs, names with dots become ambiguous since metadata 
            // consumer cannot figure where namespace ends and actual type name starts.
            // Therefore it is a good practice to avoid type names with dots.
            if (name.IndexOf('.') >= 0)
            {
                name = name.Replace('.', '_');
            }
            return name;
        }

        internal static string MakeFabricatedMethodName(int uniqueId)
        {
            return "<>n__FabricatedMethod" + uniqueId;
        }

        internal static string MakeLambdaCacheFieldName(int uniqueId)
        {
            return "CS$<>9__CachedAnonymousMethodDelegate" + uniqueId;
        }

        // Matches names generated by Dev11.
        internal static string MakeLocalName(SynthesizedLocalKind kind, int uniqueId)
        {
            Debug.Assert(kind.IsLongLived());

            if (kind == SynthesizedLocalKind.CachedAnonymousMethodDelegate)
            {
                // TODO: consider removing this special case, EE doesn't depend on the name. 
                return SynthesizedLocalNamePrefix + "<>9__CachedAnonymousMethodDelegate" + uniqueId;
            }

            if (kind == SynthesizedLocalKind.LambdaDisplayClass)
            {
                // Lambda display class local follows a different naming pattern.
                // EE depends on the name format. 
                return MakeLambdaDisplayClassStorageName(uniqueId);
            }

            return string.Format(SynthesizedLocalNamePrefix + "{0}${1:0000}", (int)kind, uniqueId);
        }

        internal static string MakeLambdaDisplayClassStorageName(int uniqueId)
        {
            return SynthesizedLocalNamePrefix + "<>8__locals" + uniqueId;
        }

        internal static bool TryParseLocalName(string name, out SynthesizedLocalKind kind, out int uniqueId)
        {
            if (name.StartsWith(SynthesizedLocalNamePrefix, StringComparison.Ordinal))
            {
                name = name.Substring(SynthesizedLocalNamePrefix.Length);
                int separator = name.IndexOf('$');
                if (separator > 0)
                {
                    int k;
                    int n;
                    if (int.TryParse(name.Substring(0, separator), NumberStyles.None, CultureInfo.InvariantCulture, out k) &&
                        int.TryParse(name.Substring(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out n))
                    {
                        kind = (SynthesizedLocalKind)k;
                        uniqueId = n;
                        return true;
                    }
                }
            }

            kind = SynthesizedLocalKind.None;
            uniqueId = 0;
            return false;
        }

        internal static string MakeFixedFieldImplementationName(string fieldName)
        {
            // the native compiler adds numeric digits at the end.  Roslyn does not.
            return "<" + fieldName + ">" + "e__FixedBuffer";
        }

        internal static string MakeStateMachineStateName()
        {
            // Microsoft.VisualStudio.VIL.VisualStudioHost.AsyncReturnStackFrame depends on this name.
            return "<>1__state";
        }

        internal static bool TryParseIteratorName(string mangledTypeName, out string iteratorName)
        {
            GeneratedNameKind kind;
            int openBracketOffset;
            int closeBracketOffset;
            if (TryParseGeneratedName(mangledTypeName, out kind, out openBracketOffset, out closeBracketOffset) &&
                (kind == GeneratedNameKind.StateMachineType) &&
                (openBracketOffset == 0))
            {
                iteratorName = mangledTypeName.Substring(openBracketOffset + 1, closeBracketOffset - openBracketOffset - 1);
                return true;
            }

            iteratorName = null;
            return false;
        }

        internal static string MakeIteratorCurrentBackingFieldName()
        {
            return "<>2__current";
        }

        internal static string MakeIteratorCurrentThreadIdName()
        {
            return "<>l__initialThreadId";
        }

        internal static string MakeIteratorFieldName(string localName, int localNumber)
        {
            return "<" + localName + ">5__" + localNumber;
        }

        internal static string IteratorThisProxyName()
        {
            return "<>4__this";
        }

        internal static string IteratorParameterProxyName(string parameterName)
        {
            return "<>3__" + parameterName;
        }

        internal static string IteratorThisProxyProxyName()
        {
            return IteratorParameterProxyName(IteratorThisProxyName());
        }

        internal static string MakeDynamicCallSiteContainerName(string methodName, int uniqueId)
        {
            methodName = EnsureNoDotsInTypeName(methodName);

            return "<" + methodName + ">o__SiteContainer" + uniqueId;
        }

        internal static string MakeDynamicCallSiteFieldName(int uniqueId)
        {
            return "<>p__Site" + uniqueId;
        }

        internal static string AsyncBuilderFieldName()
        {
            // Microsoft.VisualStudio.VIL.VisualStudioHost.AsyncReturnStackFrame depends on this name.
            return "<>t__builder";
        }

        internal static string AsyncAwaiterFieldName(int number)
        {
            return "<>u__$awaiter" + number;
        }

        internal static string SpillTempFieldName(int number)
        {
            return "<>7__wrap" + number;
        }
    }
}
