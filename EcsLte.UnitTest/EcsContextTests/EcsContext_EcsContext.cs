using System;
using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
	[TestClass]
	public class EcsContext_EcsContext : BasePrePostTest
	{
		[TestMethod]
		public void DefaultCommand()
		{
			// Has default entity command queue
			Assert.IsTrue(_context.DefaultCommand != null);
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.IsTrue(_context.DefaultCommand != null);
		}

		[TestMethod]
		public void AddUniqueComponent()
		{
			var component = new TestUniqueComponent1 { Prop = 1 };

			_context.AddUniqueComponent(component);

			// Correct component
			Assert.IsTrue(
				_context.GetUniqueComponent<TestUniqueComponent1>().Prop == component.Prop);
			// Already has component
			Assert.ThrowsException<EntityAlreadyHasUniqueComponentException>(() =>
				_context.AddUniqueComponent(component));
			// Unable to add same unique component to another entity
			Assert.ThrowsException<EntityAlreadyHasUniqueComponentException>(() =>
				_context.AddUniqueComponent(component));
			// Can add to another entity once exisitng unique component is removed
			_context.RemoveUniqueComponent<TestUniqueComponent1>();
			_context.AddUniqueComponent(component);
			Assert.IsTrue(_context.HasUniqueComponent<TestUniqueComponent1>());
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.AddUniqueComponent(new TestUniqueComponent1()));
		}

		[TestMethod]
		public void ReplaceUniqueComponent()
		{
			var component1 = new TestUniqueComponent1 { Prop = 1 };
			_context.AddUniqueComponent(new TestUniqueComponent1());

			_context.ReplaceUniqueComponent(component1);

			// Correct component
			Assert.IsTrue(
				_context.GetUniqueComponent<TestUniqueComponent1>().Prop == component1.Prop);
			// Also adds component
			var component2 = new TestUniqueComponent2 { Prop = 2 };
			_context.ReplaceUniqueComponent(component2);
			Assert.IsTrue(
				_context.GetUniqueComponent<TestUniqueComponent2>().Prop == component2.Prop);
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.ReplaceUniqueComponent(new TestUniqueComponent1()));
		}

		[TestMethod]
		public void RemoveUniqueComponent()
		{
			_context.AddUniqueComponent(new TestUniqueComponent1());

			_context.RemoveUniqueComponent<TestUniqueComponent1>();

			// Correctly removes component
			Assert.IsFalse(_context.HasUniqueComponent<TestUniqueComponent1>());
			// Cannot remove component entiy doesnt have
			Assert.ThrowsException<EntityNotHaveUniqueComponentException>(() =>
				_context.RemoveUniqueComponent<TestUniqueComponent1>());
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.RemoveUniqueComponent<TestUniqueComponent1>());
		}

		[TestMethod]
		public void CommandQueue()
		{
			var command1 = _context.CommandQueue("Test1");

			// Correctly command queue
			Assert.IsTrue(command1 != null);
			// Get same command queue
			var command2 = _context.CommandQueue("Test1");
			Assert.IsTrue(command1 == command2);
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.CommandQueue("Test1"));
		}

		[TestMethod]
		public void FilterBy()
		{
			var filter1 = _context.FilterBy(Filter.AllOf<TestComponent1>());

			// Created group
			Assert.IsTrue(filter1 != null);
			// Get same group
			var filter2 = _context.FilterBy(Filter.AllOf<TestComponent1>());
			Assert.IsTrue(filter1 == filter2);
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.FilterBy(Filter.AllOf<TestComponent1>()));
		}

		[TestMethod]
		public void GroupWith_SharedComponent()
		{
			var component = new TestSharedComponent1 { Prop = 1 };
			var group = _context.GroupWith(new TestSharedComponent1 { Prop = 1 });

			// Correct group
			Assert.IsTrue(group != null);
			Assert.IsTrue(_context.GroupWith(component) == group);
			// Different component gets different entity
			Assert.IsTrue(_context.GroupWith(new TestSharedComponent1 { Prop = 2 }) != group);
			// Null component
			ISharedComponent nullComponent = null;
			Assert.ThrowsException<ArgumentNullException>(() =>
				_context.GroupWith(nullComponent));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.GroupWith(component));
		}

		[TestMethod]
		public void GroupWith_SharedComponents()
		{
			var component1 = new TestSharedComponent1 { Prop = 1 };
			var component2 = new TestSharedComponent2 { Prop = 1 };
			var group = _context.GroupWith(component1, component2);

			// Correct group
			Assert.IsTrue(group != null);
			Assert.IsTrue(_context.GroupWith(component1, component2) == group);
			// Null component
			ISharedComponent nullComponent = null;
			Assert.ThrowsException<ArgumentNullException>(() =>
				_context.GroupWith(component1, component2, nullComponent));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.GroupWith(component1, component2));
		}

		[TestMethod]
		public void FilterByGroupWith_SharedComponent()
		{
			var component = new TestSharedComponent1 { Prop = 1 };
			var filter = Filter.AllOf<TestSharedComponent1>();
			var filterGroup = _context.FilterByGroupWith(filter, component);

			// Correct filterGroup
			Assert.IsTrue(filterGroup != null);
			Assert.IsTrue(_context.FilterByGroupWith(filter, component) == filterGroup);
			// Different component gets different entity
			Assert.IsTrue(_context
				.FilterByGroupWith(filter, new TestSharedComponent1 { Prop = 2 }) != filterGroup);
			// Null component
			ISharedComponent nullComponent = null;
			Assert.ThrowsException<ArgumentNullException>(() =>
				_context.FilterByGroupWith(filter, nullComponent));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.FilterByGroupWith(filter, component));
		}

		[TestMethod]
		public void FilterByGroupWith_SharedComponents()
		{
			var component1 = new TestSharedComponent1 { Prop = 1 };
			var component2 = new TestSharedComponent2 { Prop = 1 };
			var filter = Filter.AllOf<TestSharedComponent1>();
			var filterGroup = _context.FilterByGroupWith(filter, component1, component2);

			// Correct filterGroup
			Assert.IsTrue(filterGroup != null);
			Assert.IsTrue(_context.FilterByGroupWith(filter, component1, component2) == filterGroup);
			// Null component
			ISharedComponent nullComponent = null;
			Assert.ThrowsException<ArgumentNullException>(() =>
				_context.FilterByGroupWith(filter, component1, component2, nullComponent));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.FilterByGroupWith(filter, component1, component2));
		}
	}
}