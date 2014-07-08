﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// Kind of a synthesized local variable.
    /// </summary>
    /// <remarks>
    /// Synthesized local variables are either 
    /// 1) Short-lived (temporary)
    ///    The lifespan of an temporary variable shall not cross a statement boundary (a PDB sequence point).
    ///    These variables are not tracked by EnC and don't have names.
    ///  
    /// 2) Long-lived
    ///    All variables whose lifespan might cross a statement boundary (include a PDB sequence point)
    ///    must be named in a build configuration that supports EnC. Some of them might need to be named in release, to support EE.
    ///    The kind of such local must be encoded in the name, so that we can retrieve it from debug metadata during EnC.
    /// 
    ///    The integer value of the kind must match corresponding Dev11/12 TEMP_KIND enum values for
    ///    compatibility with assemblies generated by the native compiler.
    /// 
    ///    Long-lived local variables must be assigned slots in source order.
    /// </remarks>
    internal enum SynthesizedLocalKind : short
    {
        /// <summary>
        /// Local that stores an expression value which needs to be spilled.
        /// This local should either be lifted or it's lifespan ends before 
        /// the end of the containing await expression.
        /// </summary>
        AwaitSpilledTemp = -4,

        /// <summary>
        /// Temp variable created by the optimizer.
        /// </summary>
        OptimizerTemp = -3,

        /// <summary>
        /// Temp variable created during lowering.
        /// </summary>
        LoweringTemp = -2,

        /// <summary>
        /// The variable is not synthesized.
        /// </summary>
        None = -1,

        // The following values have to match TEMP_KIND in the native compiler.
        FirstLongLived = 0,

        /// <summary>
        /// Variable holding on the object being locked while the execution is within the block of the <see cref="LockStatementSyntax"/>.
        /// </summary>
        Lock = 2,

        /// <summary>
        /// Local variable that stores the resources to be disposed at the end of using block.
        /// </summary>
        Using = 3,

        /// <summary>
        /// Local variable that stores value of an expression consumed by a subsequent conditional branch instruction that might jump across PDB sequence points.
        /// The value needs to be preserved when remapping the IL offset from old method body to new method body during EnC.
        /// A hidden sequence point also needs to be inserted at the offset where this variable is loaded to be consumed by the branch instruction.
        /// </summary>
        ConditionalBranchDiscriminator = 4,

        /// <summary>
        /// Local variable that stores the enumerator instance.
        /// </summary>
        ForEachEnumerator = 5,

        ForEachArray = 6,
        ForEachArrayIndex0 = 7,
        ForEachArrayLimit0 = ForEachArrayIndex0 + 256,

        /// <summary>
        /// Local variable that holds a pinned handle of a string passed to a fixed statement.
        /// </summary>
        FixedString = ForEachArrayLimit0 + 256,

        // The values below have no corresponding TEMP_KIND.

        /// <summary>
        /// Boolean passed to Monitor.Enter.
        /// </summary>
        LockTaken = 520,

        /// <summary>
        /// Local variable used to cache a delegate that is used in inner block (possibly a loop), 
        /// and can be reused for all iterations fo the loop.
        /// </summary>
        CachedAnonymousMethodDelegate = 521,

        /// <summary>
        /// Local variable that holds on the display class instance.
        /// </summary>
        LambdaDisplayClass = 522,

        /// <summary>
        /// Local variable that stores the return value of an async method.
        /// </summary>
        AsyncMethodReturnValue = 523,

        /// <summary>
        /// Local variable that stores the current state of the state machine while MoveNext method is executing.
        /// Used to avoid race conditions due to multiple reads from the lifted state.
        /// </summary>
        StateMachineCachedState = 524,

        TryAwaitPendingException = 525,
        TryAwaitPendingBranch = 526,
        TryAwaitPendingCatch = 527,
        TryAwaitPendingCaughtException = 528,

        /// <summary>
        /// Very special corner case involving filters, await and lambdas.
        /// </summary>
        ExceptionFilterAwaitHoistedExceptionLocal = 529
    }

    internal static class SynthesizedLocalKindExtensions
    {
        public static bool IsLongLived(this SynthesizedLocalKind kind)
        {
            return kind >= SynthesizedLocalKind.FirstLongLived;
        }

        public static bool IsNamed(this SynthesizedLocalKind kind, DebugInformationKind debugInformationKind)
        {
            if (debugInformationKind == DebugInformationKind.Full)
            {
                return IsLongLived(kind);
            }

            switch (kind) 
            {
                case SynthesizedLocalKind.LambdaDisplayClass:
                    return true;

                default:
                    return false;
            }
        }
    }
}
