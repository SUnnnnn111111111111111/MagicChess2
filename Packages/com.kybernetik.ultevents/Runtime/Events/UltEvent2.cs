﻿// UltEvents // https://kybernetik.com.au/ultevents // Copyright 2021-2024 Kybernetik //

using System;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace UltEvents
{
    /// <summary>
    /// Allows you to expose the add and remove methods of an <see cref="UltEvent{T0, T1}"/>
    /// without exposing the rest of its members such as the ability to invoke it.
    /// </summary>
    public interface IUltEvent<T0, T1> : IUltEventBase
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Delegates registered here are invoked by <see cref="UltEvent{T0, T1}.Invoke"/> after all
        /// <see cref="UltEvent{T0, T1}.PersistentCalls"/>.
        /// </summary>
        event Action<T0, T1> DynamicCalls;

        /// <summary>
        /// Invokes all <see cref="UltEvent.PersistentCalls"/> then all <see cref="DynamicCalls"/>.
        /// </summary>
        void Invoke(T0 parameter0, T1 parameter1);

        /************************************************************************************************************************/
    }

    /// <summary>A serializable event with 2 parameters which can be viewed and configured in the inspector.</summary>
    /// <remarks>This is a more versatile and user friendly implementation than <see cref="UnityEvent{T0, T1}"/>.</remarks>
    [Serializable]
    public class UltEvent<T0, T1> : UltEventBase, IUltEvent<T0, T1>
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int ParameterCount
            => 2;

        /// <inheritdoc/>
        public override Type GetParameterType(int index)
            => index switch
            {
                0 => typeof(T0),
                1 => typeof(T1),
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };

        /************************************************************************************************************************/

        /// <summary>
        /// Delegates registered to this event are serialized as <see cref="PersistentCall"/>s and are invoked by
        /// <see cref="Invoke"/> before all <see cref="DynamicCalls"/>.
        /// </summary>
        public event Action<T0, T1> PersistentCalls
        {
            add => AddPersistentCall(value);
            remove => RemovePersistentCall(value);
        }

        /************************************************************************************************************************/

        private Action<T0, T1> _DynamicCalls;

        /// <summary>
        /// Delegates registered here are invoked by <see cref="Invoke"/> after all <see cref="PersistentCalls"/>.
        /// </summary>
        public event Action<T0, T1> DynamicCalls
        {
            add
            {
                _DynamicCalls += value;
                OnDynamicCallsChanged();
            }
            remove
            {
                _DynamicCalls -= value;
                OnDynamicCallsChanged();
            }
        }

        /// <summary>
        /// The non-serialized method and parameter details of this event.
        /// Delegates registered here are called by <see cref="Invoke"/> after all <see cref="PersistentCalls"/>.
        /// </summary>
        protected override Delegate DynamicCallsBase
        {
            get => _DynamicCalls;
            set => _DynamicCalls = value as Action<T0, T1>;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Operators and Call Registration
        /************************************************************************************************************************/

        /// <summary>
        /// Ensures that `ultEvent` isn't null and adds `method` to its <see cref="PersistentCalls"/> (if in Edit Mode) or
        /// <see cref="DynamicCalls"/> (in Play Mode and at runtime).
        /// </summary>
        public static UltEvent<T0, T1> operator +(UltEvent<T0, T1> ultEvent, Action<T0, T1> method)
        {
            ultEvent ??= new UltEvent<T0, T1>();

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && method.Target is Object)
            {
                ultEvent.PersistentCalls += method;
                return ultEvent;
            }
#endif

            ultEvent.DynamicCalls += method;
            return ultEvent;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If `ultEvent` isn't null, this method removes `method` from its <see cref="PersistentCalls"/> (if in Edit Mode) or
        /// <see cref="DynamicCalls"/> (in Play Mode and at runtime).
        /// </summary>
        public static UltEvent<T0, T1> operator -(UltEvent<T0, T1> ultEvent, Action<T0, T1> method)
        {
            if (ultEvent == null)
                return null;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && method.Target is Object)
            {
                ultEvent.PersistentCalls -= method;
                return ultEvent;
            }
#endif

            ultEvent.DynamicCalls -= method;
            return ultEvent;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates a new <see cref="UltEventBase"/> and adds `method` to its <see cref="PersistentCalls"/> (if in edit
        /// mode), or <see cref="DynamicCalls"/> (in Play Mode and at runtime).
        /// </summary>
        public static implicit operator UltEvent<T0, T1>(Action<T0, T1> method)
        {
            if (method == null)
                return null;

            var ultEvent = new UltEvent<T0, T1>();
            ultEvent += method;
            return ultEvent;
        }

        /************************************************************************************************************************/

        /// <summary>Ensures that `ultEvent` isn't null and adds `method` to its <see cref="DynamicCalls"/>.</summary>
        public static void AddDynamicCall(ref UltEvent<T0, T1> ultEvent, Action<T0, T1> method)
        {
            ultEvent ??= new UltEvent<T0, T1>();

            ultEvent.DynamicCalls += method;
        }

        /// <summary>If `ultEvent` isn't null, this method removes `method` from its <see cref="DynamicCalls"/>.</summary>
        public static void RemoveDynamicCall(ref UltEvent<T0, T1> ultEvent, Action<T0, T1> method)
        {
            if (ultEvent != null)
                ultEvent.DynamicCalls -= method;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public virtual void Invoke(T0 parameter0, T1 parameter1)
        {
            CacheParameter(parameter0);
            CacheParameter(parameter1);
            InvokePersistentCalls();
            _DynamicCalls?.Invoke(parameter0, parameter1);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Invokes all <see cref="PersistentCalls"/> then all <see cref="DynamicCalls"/>
        /// inside a try/catch block which logs any exceptions that are thrown.
        /// </summary>
        public virtual void InvokeSafe(T0 parameter0, T1 parameter1)
        {
            try
            {
                Invoke(parameter0, parameter1);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        /************************************************************************************************************************/
    }
}
