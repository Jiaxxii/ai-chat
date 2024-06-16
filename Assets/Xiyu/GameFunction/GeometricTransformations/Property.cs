namespace Xiyu.GameFunction.GeometricTransformations
{
    public delegate void Setter<in T>(T value);

    public delegate T Getter<out T>();


    public readonly struct Property<T>
    {
        private readonly Getter<T> _getterProperty;
        private readonly Setter<T> _setterProperty;

        public T Member
        {
            get => GetValue();
            set => SetValue(value);
        }

        public Property(Getter<T> getter, Setter<T> setter)
        {
            _getterProperty = getter;
            _setterProperty = setter;
        }


        public T GetValue() => _getterProperty.Invoke() ?? default;

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


        public void SetValue(T value) => _setterProperty.Invoke(value);

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