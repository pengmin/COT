using System.Collections.Generic;

namespace Cot.Infrastructure
{
	/// <summary>
	/// 操作结果
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	public class ActionResult<TResult>
	{
		/// <summary>
		/// 结果对象
		/// </summary>
		public TResult Result { get; set; }
		/// <summary>
		/// 操作是否成功
		/// </summary>
		public bool Success { get; set; }

		private readonly List<string> _message;
		/// <summary>
		/// 消息
		/// </summary>
		public IList<string> Message { get { return _message; } }

		public ActionResult()
		{
			_message = new List<string>();
		}
	}
}
