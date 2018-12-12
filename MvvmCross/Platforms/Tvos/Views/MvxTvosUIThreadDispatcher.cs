﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using MvvmCross.Base;
using MvvmCross.Exceptions;
using UIKit;
using static MvvmCross.Base.MvxAsyncPump;

namespace MvvmCross.Platforms.Tvos.Views
{
    public abstract class MvxTvosUIThreadDispatcher
        : MvxMainThreadAsyncDispatcher
    {
        private readonly SynchronizationContext _uiSynchronizationContext;

        protected MvxTvosUIThreadDispatcher()
        {
            _uiSynchronizationContext = SynchronizationContext.Current;
            if (_uiSynchronizationContext == null)
                throw new MvxException("SynchronizationContext must not be null - check to make sure Dispatcher is created on UI thread");
        }

        public override bool RequestMainThreadAction(Action action, bool maskExceptions = true)
        {
            if (IsOnMainThread)
                ExceptionMaskedAction(action, maskExceptions);
            else
                UIApplication.SharedApplication.BeginInvokeOnMainThread(() =>
            {
                ExceptionMaskedAction(action, maskExceptions);
            });
            return true;
        }

        public override bool IsOnMainThread
        {
            get
            {
                if (_uiSynchronizationContext == SynchronizationContext.Current)
                    return true;

                if (SynchronizationContext.Current is SingleThreadSynchronizationContext)
                    return true;

                return false;
            }
        }
    }
}
