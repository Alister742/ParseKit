// Type: System.Net.GlobalProxySelection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\System.dll

using System;
using System.Runtime;

namespace System.Net
{
  /// <summary>
  /// Содержит глобальный экземпляр прокси-сервера по умолчанию для всех HTTP-запросов.
  /// </summary>
  [Obsolete("This class has been deprecated. Please use WebRequest.DefaultWebProxy instead to access and set the global default proxy. Use 'null' instead of GetEmptyWebProxy. http://go.microsoft.com/fwlink/?linkid=14202")]
  public class GlobalProxySelection
  {
    /// <summary>
    /// Возвращает или устанавливает глобальный прокси-сервер HTTP.
    /// </summary>
    /// 
    /// <returns>
    /// Объект <see cref="T:System.Net.IWebProxy"/>, используемый всеми вызовами <see cref="M:System.Net.HttpWebRequest.GetResponse"/>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Для операции задания было указано значение null. </exception><exception cref="T:System.Security.SecurityException">Вызывающий оператор не имеет разрешения на запрашиваемую операцию. </exception>
    public static IWebProxy Select
    {
      get
      {
        IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
        if (defaultWebProxy == null)
          return GlobalProxySelection.GetEmptyWebProxy();
        WebRequest.WebProxyWrapper webProxyWrapper = defaultWebProxy as WebRequest.WebProxyWrapper;
        if (webProxyWrapper != null)
          return (IWebProxy) webProxyWrapper.WebProxy;
        else
          return defaultWebProxy;
      }
      set
      {
        WebRequest.DefaultWebProxy = value;
      }
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Net.GlobalProxySelection"/>.
    /// </summary>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public GlobalProxySelection()
    {
    }

    /// <summary>
    /// Возвращает пустой экземпляр прокси-сервера.
    /// </summary>
    /// 
    /// <returns>
    /// Объект <see cref="T:System.Net.IWebProxy"/>, не содержащий данных.
    /// </returns>
    public static IWebProxy GetEmptyWebProxy()
    {
      return (IWebProxy) new EmptyWebProxy();
    }
  }
}
