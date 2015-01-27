using System;

namespace Cot.Infrastructure
{
	/// <summary>
	/// 实体类基类
	/// </summary>
	public abstract class Entity
	{
	}
	/// <summary>
	/// 实体类基类
	/// </summary>
	/// <typeparam name="TId">Id类型</typeparam>
	public abstract class Entity<TId> : Entity
	{
		/// <summary>
		/// Id
		/// </summary>
		public TId Id { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as Entity<TId>);
		}
		public virtual bool Equals(Entity<TId> other)
		{
			if (other == null) return false;
			if (ReferenceEquals(this, other)) return true;
			if (IsTransient(this) || IsTransient(other) || !Equals(Id, other.Id)) return false;

			var otherType = other.GetUnproxiedType();
			var thisType = GetUnproxiedType();
			return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
		}
		public static bool operator ==(Entity<TId> x, Entity<TId> y)
		{
			return Equals(x, y);
		}
		public static bool operator !=(Entity<TId> x, Entity<TId> y)
		{
			return !(x == y);
		}
		public override int GetHashCode()
		{
			return Equals(Id, default(TId)) ? base.GetHashCode() : Id.GetHashCode();
		}

		private static bool IsTransient(Entity<TId> obj)
		{
			return obj != null && Equals(obj.Id, default(TId));
		}
		private Type GetUnproxiedType()
		{
			return GetType();
		}
	}
}
