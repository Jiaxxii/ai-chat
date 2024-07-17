using System;

namespace Xiyu.GameFunction.GeometricTransformations
{
    public delegate void Setter<in T>(T value);

    public delegate T Getter<out T>();


    public readonly struct Property<T>
    {
        private readonly Getter<T> _getterProperty;
        private readonly Setter<T> _setterProperty;

        private readonly Action<T> _onSetterValueChange;

        public T Member
        {
            get => GetValue();
            set => SetValue(value);
        }

        public Property(Getter<T> getter, Setter<T> setter)
        {
            _getterProperty = getter;
            _setterProperty = setter;
            _onSetterValueChange = null;
        }

        public Property(Getter<T> getter, Setter<T> setter, Action<T> onSetterValueChange)
        {
            _getterProperty = getter;
            _setterProperty = setter;
            _onSetterValueChange = onSetterValueChange;
        }

        public T GetValue() => _getterProperty.Invoke() ?? default;

        [Obsolete("我们认为 TryGetValue 是多余的，判断是否存在 [Get/Set]ter 使用 Has[Get/Set]terProperty 足够。", false)]
        public bool TryGetValue(out T value)
        {
            if (_getterProperty is null)
            {
                value = default;
                return false;
            }

            value = _getterProperty.Invoke();
            return true;
        }


        public void SetValue(T value)
        {
            _setterProperty.Invoke(value);
            _onSetterValueChange?.Invoke(value);
        }

        [Obsolete("判断是否存在 [Get/Set]ter 建议使用 Has[Get/Set]terProperty。", false)]
        public bool TrySetValue(T value)
        {
            if (_setterProperty is null)
            {
                return false;
            }

            _setterProperty.Invoke(value);
            return true;
        }

        public bool HasGetterProperty() => _getterProperty is not null;
        public bool HasSetterProperty() => _setterProperty is not null;
    }
}