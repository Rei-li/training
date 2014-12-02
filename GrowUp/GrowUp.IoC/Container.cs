using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowUp.IoC
{

    class Container
    {
        private readonly IDictionary<Type, RegisteredObject> _registeredObjects =
            new Dictionary<Type, RegisteredObject>();


        public void Register<TType, TConcrete>() where TConcrete : class, TType
        {
            Register<TType, TConcrete>(false, null);
        }

        public object Resolve(Type type)
        {
            return ResolveObject(type);
        }

        private void Register<TType, TConcrete>(bool isSingleton, TConcrete instance)
        {
            Type type = typeof(TType);

            if (_registeredObjects.ContainsKey(type))

                _registeredObjects.Remove(type);

            _registeredObjects.Add(type, new RegisteredObject(typeof(TConcrete), isSingleton, instance));

        }

        private object ResolveObject(Type type)
        {
            var registeredObject = _registeredObjects[type];

            if (registeredObject == null)
            {
                throw new ArgumentOutOfRangeException(string.Format("Тип {0} Не зарегистрирован", type.Name));
            }
            return GetInstance(registeredObject);
        }

        private object GetInstance(RegisteredObject registeredObject)
        {
            object instance = registeredObject.SingletonInstance;

            if (instance == null)
            {
                var parameters = ResolveConstructorParameters(registeredObject);
                instance = registeredObject.CreateInstance(parameters.ToArray());
            }
            return instance;
        }

        private IEnumerable<object> ResolveConstructorParameters(RegisteredObject registeredObject)
        {
            var constructorInfo = registeredObject.ConcreteType.GetConstructors().First();
            return constructorInfo.GetParameters().Select(parameter => ResolveObject(parameter.ParameterType));
        }

        private class RegisteredObject
        {
            private readonly bool _isSinglton;

            public RegisteredObject(Type concreteType, bool isSingleton, object instance)
            {
                _isSinglton = isSingleton;
                ConcreteType = concreteType;
                SingletonInstance = instance;
            }

            public Type ConcreteType { get; private set; }
            public object SingletonInstance { get; private set; }
            public object CreateInstance(params object[] args)
            {
                object instance = Activator.CreateInstance(ConcreteType, args);
                if (_isSinglton)
                    SingletonInstance = instance;
                return instance;
            }
        }
    }
}
