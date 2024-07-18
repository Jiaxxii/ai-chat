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

        /// <summary>
        /// 公共属性Member，提供对值的访问
        /// </summary>
        public T Member
        {
            get => GetValue();
            set => SetValue(value);
        }

        /// <summary>
        /// 构造函数，初始化getter和setter
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        public Property(Getter<T> getter, Setter<T> setter)
        {
            _getterProperty = getter;
            _setterProperty = setter;
            _onSetterValueChange = null;
        }

        /// <summary>
        /// 构造函数，初始化getter、setter和值变更回调
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="onSetterValueChange"></param>
        public Property(Getter<T> getter, Setter<T> setter, Action<T> onSetterValueChange)
        {
            _getterProperty = getter;
            _setterProperty = setter;
            _onSetterValueChange = onSetterValueChange;
        }
        
        /// <summary>
        /// 获取当前属性的值，如果getter未设置则抛出NullReferenceException异常
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">未设置引用</exception>
        public T GetValue() => _getterProperty.Invoke() ?? throw new NullReferenceException($"{nameof(_getterProperty)}未设置引用");


        /// <summary>
        /// 设置当前属性的值，并触发值变更回调（如果有的话）
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value)
        {
            _setterProperty.Invoke(value);
            _onSetterValueChange?.Invoke(value);
        }
        
        /// <summary>
        /// 检查是否设置了getter
        /// </summary>
        /// <returns></returns>
        public bool HasGetterProperty() => _getterProperty is not null;
        
        /// <summary>
        /// 检查是否设置了setter
        /// </summary>
        /// <returns></returns>
        public bool HasSetterProperty() => _setterProperty is not null;
    }
}