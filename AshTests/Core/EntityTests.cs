﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Net.RichardLord.Ash.Core;

namespace Net.RichardLord.AshTests.Core
{
    [TestFixture]
	public class EntityTests
	{
        private Entity _entity;

        [SetUp]
		public void CreateEntity()
		{
			_entity = new Entity();
		}

		[TearDown]
		public void ClearEntity()
		{
			_entity = null;
		}

		[Test]
		public void AddReturnsReferenceToEntity()
		{
			var component = new MockComponent();
			var e = _entity.Add(component);
		    Assert.AreSame(_entity, e);
		}

        [Test]
        public void CanStoreAndRetrieveComponent()
        {
            var component = new MockComponent();
            _entity.Add(component);
            Assert.AreSame(_entity.Get(typeof(MockComponent)), component);
        }

        [Test]
        public void CanStoreAndRetrieveMultipleComponents()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponent2();
            _entity.Add(component2);
            Assert.AreSame(_entity.Get(typeof(MockComponent)), component1);
            Assert.AreSame(_entity.Get(typeof(MockComponent2)), component2);
        }

        [Test]
        public void CanReplaceComponent()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponent();
            _entity.Add(component2);
            Assert.AreEqual(_entity.Get(typeof(MockComponent)), component2);
        }

        [Test]
        public void CanStoreBaseAndExtendedComponents()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponentExtended();
            _entity.Add(component2);
            Assert.AreEqual(_entity.Get(typeof(MockComponent)), component1);
            Assert.AreEqual(_entity.Get(typeof(MockComponentExtended)), component2);
        }

        [Test]
        public void CanStoreExtendedComponentAsBaseType()
        {
            var component = new MockComponentExtended();
            _entity.Add(component, typeof(MockComponent));
            Assert.AreEqual(_entity.Get(typeof(MockComponent)), component);
        }

        [Test]
        public void GetReturnNullIfNoComponent()
        {
            Assert.IsNull(_entity.Get(typeof(MockComponent)));
        }

        [Test]
        public void WillRetrieveAllComponents()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponent2();
            _entity.Add(component2);
            var all = _entity.GetAll();
            Assert.AreEqual(new List<object> { component1, component2 }, all);
        }

        [Test]
        public void HasComponentIsFalseIfComponentTypeNotPresent()
        {
            _entity.Add(new MockComponent2());
            Assert.IsFalse(_entity.Has(typeof(MockComponent)));
        }

        [Test]
        public void HasComponentIsTrueIfComponentTypeIsPresent()
        {
            _entity.Add(new MockComponent());
            Assert.IsTrue(_entity.Has(typeof(MockComponent)));
        }

        [Test]
        public void CanRemoveComponent()
        {
            var component = new MockComponent();
            _entity.Add(component);
            _entity.Remove(typeof(MockComponent));
            Assert.IsFalse(_entity.Has(typeof(MockComponent)));
        }

        [Test]
        public void StoringComponentTriggersAddedSignal()
        {
            var eventFired = false;
            var component = new MockComponent();
            _entity.ComponentAdded += (entity, componentType) => eventFired = true;
            _entity.Add(component);
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void RemovingComponentTriggersRemovedSignal()
        {
            var eventFired = false;
            var component = new MockComponent();
            _entity.Add(component);
            _entity.ComponentRemoved += (entity, componentType) => eventFired = true;
            _entity.Remove(typeof(MockComponent));
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void ComponentAddedSignalContainsCorrectType()
        {
            Type type = null;
            var component = new MockComponent();
            _entity.ComponentAdded += (entity, componentType) => type = componentType;
            _entity.Add(component);
            Assert.AreEqual(typeof(MockComponent), type);
        }

        [Test]
        public void ComponentAddedSignalEnablesAccessToComponentValue()
        {
            var value = 0;
            var component = new MockComponent { Value = 10 };
            _entity.ComponentAdded += 
                (entity, componentType) => value = ((MockComponent)entity.Get(componentType)).Value;
            _entity.Add(component);
            Assert.AreEqual(10, value);
        }

        [Test]
        public void ComponentRemovedSignalContainsCorrectType()
        {
            Type type = null;
            var component = new MockComponent();
            _entity.ComponentRemoved += (entity, componentType) => type = componentType;
            _entity.Add(component);
            _entity.Remove(typeof(MockComponent));
            Assert.AreEqual(typeof(MockComponent), type);
        }

        [Test]
        public void ComponentRemovedSignalContainsEntityWithRemovedComponent()
        {
            var component = new MockComponent { Value = 10 };
            _entity.ComponentRemoved +=
                (entity, componentType) => component = (MockComponent)entity.Get(componentType);
            _entity.Add(component);
            _entity.Remove(typeof (MockComponent));
            Assert.IsNull(component);
        }

        [Test]
        public void CloneIsNewReference()
        {
            _entity.Add(new MockComponent());
            var clone = _entity.Clone();
            Assert.AreNotEqual(clone, _entity);
        }

        [Test]
        public void CloneHasChildComponent()
        {
            _entity.Add(new MockComponent());
            var clone = _entity.Clone();
            Assert.IsTrue(clone.Has(typeof(MockComponent)));
        }

        [Test]
        public void CloneChildComponentIsNewReference()
        {
            _entity.Add(new MockComponent());
            var clone = _entity.Clone();
            Assert.AreNotEqual(_entity.Get(typeof(MockComponent)), clone.Get(typeof(MockComponent)));
        }

        [Test]
        public void CloneChildComponentHasSameProperties()
        {
            var component = new MockComponent {Value = 5};
            _entity.Add(component);
            var clone = _entity.Clone();
            Assert.AreEqual(5, clone.Get<MockComponent>(typeof(MockComponent)).Value);
        }
	}

    class MockComponent
    {
        public int Value { get; set; }
    }

    class MockComponent2 {}

    class MockComponentExtended : MockComponent {}
}

