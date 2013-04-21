// Type: System.IAsyncResult
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
  /// <summary>
  /// Представляет состояние асинхронной операции.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  [ComVisible(true)]
  public interface IAsyncResult
  {
    /// <summary>
    /// Возвращает значение, показывающее, выполнена ли асинхронная операция.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если операция завершена, в противном случае — значение false.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    bool IsCompleted { get; }

    /// <summary>
    /// Возвращает дескриптор <see cref="T:System.Threading.WaitHandle"/>, используемый для режима ожидания завершения асинхронной операции.
    /// </summary>
    /// 
    /// <returns>
    /// Объект <see cref="T:System.Threading.WaitHandle"/>, используемый для режима ожидания завершения асинхронной операции.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    WaitHandle AsyncWaitHandle { get; }

    /// <summary>
    /// Возвращает определенный пользователем объект, который определяет или содержит в себе сведения об асинхронной операции.
    /// </summary>
    /// 
    /// <returns>
    /// Определенный пользователем объект, который определяет или содержит в себе сведения об асинхронной операции.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    object AsyncState { get; }

    /// <summary>
    /// Возвращает значение, показывающее, синхронно ли закончилась асинхронная операция.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если асинхронная операция завершилась синхронно, в противном случае — значение false.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    bool CompletedSynchronously { get; }
  }
}
