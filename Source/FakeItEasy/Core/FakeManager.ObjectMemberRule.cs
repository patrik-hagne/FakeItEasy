﻿namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Creation;

    public partial class FakeManager
    {
        [Serializable]
        private class ObjectMemberRule
            : IFakeObjectCallRule
        {
            private static readonly List<RuntimeMethodHandle> objectMethodsMethodHandles =
                new List<RuntimeMethodHandle>
                    {
                        typeof(object).GetMethod("Equals", new[] { typeof(object) }).MethodHandle, 
                        typeof(object).GetMethod("ToString", new Type[] { }).MethodHandle, 
                        typeof(object).GetMethod("GetHashCode", new Type[] { }).MethodHandle
                    };

            public FakeManager FakeManager { get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return IsObjetMethod(fakeObjectCall);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                if (this.TryHandleToString(fakeObjectCall))
                {
                    return;
                }

                if (this.TryHandleGetHashCode(fakeObjectCall))
                {
                    return;
                }

                if (this.TryHandleEquals(fakeObjectCall))
                {
                    return;
                }
            }

            private static bool IsObjetMethod(IFakeObjectCall fakeObjectCall)
            {
                return objectMethodsMethodHandles.Contains(fakeObjectCall.Method.MethodHandle);
            }

            private bool TryHandleGetHashCode(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.MethodHandle.Equals(objectMethodsMethodHandles[2]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(this.FakeManager.GetHashCode());

                return true;
            }

            private bool TryHandleToString(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.MethodHandle.Equals(objectMethodsMethodHandles[1]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue("Faked {0}".FormatInvariant(this.FakeManager.FakeObjectType.FullName));

                return true;
            }

            private bool TryHandleEquals(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.MethodHandle.Equals(objectMethodsMethodHandles[0]))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0] as ITaggable;

                if (argument != null)
                {
                    fakeObjectCall.SetReturnValue(argument.Tag.Equals(this.FakeManager));
                }
                else
                {
                    fakeObjectCall.SetReturnValue(false);
                }

                return true;
            }
        }
    }
}