// Type: System.Net.CookieContainer
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\System.dll

using System;
using System.Collections;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime;
using System.Text;
using System.Threading;

namespace System.Net
{
  /// <summary>
  /// Предоставляет контейнер для коллекции объектов <see cref="T:System.Net.CookieCollection"/>.
  /// </summary>
  [Serializable]
  public class CookieContainer
  {
    private static readonly HeaderVariantInfo[] HeaderInfo = new HeaderVariantInfo[2]
    {
      new HeaderVariantInfo("Set-Cookie", CookieVariant.Rfc2109),
      new HeaderVariantInfo("Set-Cookie2", CookieVariant.Rfc2965)
    };
    private Hashtable m_domainTable = new Hashtable();
    private int m_maxCookieSize = 4096;
    private int m_maxCookies = 300;
    private int m_maxCookiesPerDomain = 20;
    private string m_fqdnMyDomain = string.Empty;
    /// <summary>
    /// Представляет используемое по умолчанию максимальное число экземпляров <see cref="T:System.Net.Cookie"/>, которые могут храниться в <see cref="T:System.Net.CookieContainer"/>.Это поле имеет постоянное значение.
    /// </summary>
    public const int DefaultCookieLimit = 300;
    /// <summary>
    /// Представляет используемое по умолчанию максимальное число экземпляров <see cref="T:System.Net.Cookie"/>, которые могут храниться в <see cref="T:System.Net.CookieContainer"/> для каждого домена.Это поле имеет постоянное значение.
    /// </summary>
    public const int DefaultPerDomainCookieLimit = 20;
    /// <summary>
    /// Представляет используемый по умолчанию максимальный размер в байтах экземпляров <see cref="T:System.Net.Cookie"/>, которые могут храниться в <see cref="T:System.Net.CookieContainer"/>.Это поле имеет постоянное значение.
    /// </summary>
    public const int DefaultCookieLengthLimit = 4096;
    private int m_count;

    /// <summary>
    /// Возвращает и задает количество экземпляров <see cref="T:System.Net.Cookie"/>, которое может храниться в <see cref="T:System.Net.CookieContainer"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Количество экземпляров <see cref="T:System.Net.Cookie"/>, которое может храниться в <see cref="T:System.Net.CookieContainer"/>.Это строгое ограничение и не может превышаться путем добавления <see cref="T:System.Net.Cookie"/>.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="Capacity"/> меньше или равно нулю или (значение меньше <see cref="P:System.Net.CookieContainer.PerDomainCapacity"/> и <see cref="P:System.Net.CookieContainer.PerDomainCapacity"/> не равно <see cref="F:System.Int32.MaxValue"/>). </exception>
    public int Capacity
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.m_maxCookies;
      }
      set
      {
        if (value <= 0 || value < this.m_maxCookiesPerDomain && this.m_maxCookiesPerDomain != int.MaxValue)
        {
          throw new ArgumentOutOfRangeException("value", SR.GetString("net_cookie_capacity_range", (object) "Capacity", (object) 0, (object) this.m_maxCookiesPerDomain));
        }
        else
        {
          if (value < this.m_maxCookies)
          {
            this.m_maxCookies = value;
            this.AgeCookies((string) null);
          }
          this.m_maxCookies = value;
        }
      }
    }

    /// <summary>
    /// Возвращает количество экземпляров <see cref="T:System.Net.Cookie"/>, хранящихся в текущий момент в <see cref="T:System.Net.CookieContainer"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Количество экземпляров <see cref="T:System.Net.Cookie"/>, хранящихся в текущий момент в <see cref="T:System.Net.CookieContainer"/>.Это общее число экземпляров <see cref="T:System.Net.Cookie"/> во всех доменах.
    /// </returns>
    public int Count
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.m_count;
      }
    }

    /// <summary>
    /// Представляет наибольшую допустимую длину <see cref="T:System.Net.Cookie"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Наибольшая допустимая длина <see cref="T:System.Net.Cookie"/>, выраженная в байтах.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="MaxCookieSize"/> меньше или равно нулю. </exception>
    public int MaxCookieSize
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.m_maxCookieSize;
      }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException("value");
        this.m_maxCookieSize = value;
      }
    }

    /// <summary>
    /// Возвращает и задает количество экземпляров <see cref="T:System.Net.Cookie"/>, которое может храниться в <see cref="T:System.Net.CookieContainer"/> для каждого домена.
    /// </summary>
    /// 
    /// <returns>
    /// Допустимое количество экземпляров <see cref="T:System.Net.Cookie"/>, приходящихся на каждый домен.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="PerDomainCapacity"/> меньше или равно нулю. –или– (Параметр <paramref name="(PerDomainCapacity"/> больше максимально допустимого числа экземпляров cookies, равного 300, и не равен <see cref="F:System.Int32.MaxValue"/>). </exception>
    public int PerDomainCapacity
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.m_maxCookiesPerDomain;
      }
      set
      {
        if (value <= 0 || value > this.m_maxCookies && value != int.MaxValue)
          throw new ArgumentOutOfRangeException("value");
        if (value < this.m_maxCookiesPerDomain)
        {
          this.m_maxCookiesPerDomain = value;
          this.AgeCookies((string) null);
        }
        this.m_maxCookiesPerDomain = value;
      }
    }

    static CookieContainer()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Net.CookieContainer"/>.
    /// </summary>
    public CookieContainer()
    {
      string domainName = IPGlobalProperties.InternalGetIPGlobalProperties().DomainName;
      if (domainName == null || domainName.Length <= 1)
        return;
      this.m_fqdnMyDomain = (string) (object) '.' + (object) domainName;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Net.CookieContainer"/> с указанным значением для числа экземпляров <see cref="T:System.Net.Cookie"/>, которые может хранить контейнер.
    /// </summary>
    /// <param name="capacity">Количество экземпляров <see cref="T:System.Net.Cookie"/>, которое может храниться в <see cref="T:System.Net.CookieContainer"/>. </param><exception cref="T:System.ArgumentException">Значение параметра <paramref name="capacity"/> меньше или равно нулю. </exception>
    public CookieContainer(int capacity)
      : this()
    {
      if (capacity <= 0)
        throw new ArgumentException(SR.GetString("net_toosmall"), "Capacity");
      this.m_maxCookies = capacity;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Net.CookieContainer"/> с определенными свойствами.
    /// </summary>
    /// <param name="capacity">Количество экземпляров <see cref="T:System.Net.Cookie"/>, которое может храниться в <see cref="T:System.Net.CookieContainer"/>. </param><param name="perDomainCapacity">Количество экземпляров <see cref="T:System.Net.Cookie"/>, приходящееся на каждый домен. </param><param name="maxCookieSize">Максимальный размер в байтах для любого отдельного экземпляра <see cref="T:System.Net.Cookie"/> в <see cref="T:System.Net.CookieContainer"/>. </param><exception cref="T:System.ArgumentException">Значение параметра <paramref name="perDomainCapacity"/> не равно значению параметра <see cref="F:System.Int32.MaxValue"/>. и  <paramref name="(perDomainCapacity"/> меньше либо равен нулю или <paramref name="perDomainCapacity"/> больше <paramref name="capacity)"/>. </exception><exception cref="T:System.ArgumentException"><paramref name="maxCookieSize"/> is less than or equal to zero. </exception>
    public CookieContainer(int capacity, int perDomainCapacity, int maxCookieSize)
      : this(capacity)
    {
      if (perDomainCapacity != int.MaxValue && (perDomainCapacity <= 0 || perDomainCapacity > capacity))
      {
        throw new ArgumentOutOfRangeException("perDomainCapacity", SR.GetString("net_cookie_capacity_range", (object) "PerDomainCapacity", (object) 0, (object) capacity));
      }
      else
      {
        this.m_maxCookiesPerDomain = perDomainCapacity;
        if (maxCookieSize <= 0)
          throw new ArgumentException(SR.GetString("net_toosmall"), "MaxCookieSize");
        this.m_maxCookieSize = maxCookieSize;
      }
    }

    /// <summary>
    /// Добавляет <see cref="T:System.Net.Cookie"/> в <see cref="T:System.Net.CookieContainer"/>.В этом методе используется домен из класса <see cref="T:System.Net.Cookie"/> для определения доменной коллекции, которую требуется связать с <see cref="T:System.Net.Cookie"/>.
    /// </summary>
    /// <param name="cookie">Объект <see cref="T:System.Net.Cookie"/>, добавляемый в <see cref="T:System.Net.CookieContainer"/>. </param><exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="cookie"/> — null. </exception><exception cref="T:System.ArgumentException">Домен для <paramref name="cookie"/> равен null или пустой строке (""). </exception><exception cref="T:System.Net.CookieException">Значение <paramref name="cookie"/> больше, чем значение <paramref name="maxCookieSize"/>–или– домен для <paramref name="cookie"/> не является допустимым URI. </exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public void Add(Cookie cookie)
    {
      if (cookie == null)
        throw new ArgumentNullException("cookie");
      if (cookie.Domain.Length == 0)
        throw new ArgumentException(SR.GetString("net_emptystringcall"), "cookie.Domain");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(cookie.Secure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp).Append(Uri.SchemeDelimiter);
      if (!cookie.DomainImplicit && (int) cookie.Domain[0] == 46)
        stringBuilder.Append("0");
      stringBuilder.Append(cookie.Domain);
      if (cookie.PortList != null)
        stringBuilder.Append(":").Append(cookie.PortList[0]);
      stringBuilder.Append(cookie.Path);
      Uri result;
      if (!Uri.TryCreate(((object) stringBuilder).ToString(), UriKind.Absolute, out result))
      {
        throw new CookieException(SR.GetString("net_cookie_attribute", (object) "Domain", (object) cookie.Domain));
      }
      else
      {
        Cookie cookie1 = cookie.Clone();
        cookie1.VerifySetDefaults(cookie1.Variant, result, this.IsLocalDomain(result.Host), this.m_fqdnMyDomain, true, true);
        this.Add(cookie1, true);
      }
    }

    /// <summary>
    /// Добавляет данные <see cref="T:System.Net.CookieCollection"/> в <see cref="T:System.Net.CookieContainer"/>.
    /// </summary>
    /// <param name="cookies">The <see cref="T:System.Net.CookieCollection"/> to be added to the <see cref="T:System.Net.CookieContainer"/>. </param><exception cref="T:System.ArgumentNullException"><paramref name="cookies"/> is null. </exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public void Add(CookieCollection cookies)
    {
      if (cookies == null)
        throw new ArgumentNullException("cookies");
      foreach (Cookie cookie in cookies)
        this.Add(cookie);
    }

    /// <summary>
    /// Добавляет экземпляр <see cref="T:System.Net.Cookie"/> в <see cref="T:System.Net.CookieContainer"/> для определенного URI.
    /// </summary>
    /// <param name="uri">The URI of the <see cref="T:System.Net.Cookie"/> to be added to the <see cref="T:System.Net.CookieContainer"/>. </param><param name="cookie">The <see cref="T:System.Net.Cookie"/> to be added to the <see cref="T:System.Net.CookieContainer"/>. </param><exception cref="T:System.ArgumentNullException">Значение <paramref name="uri"/> равно null или <paramref name="cookie"/> равно null. </exception><exception cref="T:System.Net.CookieException">Значение <paramref name="cookie"/> больше, чем значение <paramref name="maxCookieSize"/>-or- Домен для <paramref name="cookie"/> не является допустимым URI. </exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public void Add(Uri uri, Cookie cookie)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException("uri");
      if (cookie == null)
        throw new ArgumentNullException("cookie");
      Cookie cookie1 = cookie.Clone();
      cookie1.VerifySetDefaults(cookie1.Variant, uri, this.IsLocalDomain(uri.Host), this.m_fqdnMyDomain, true, true);
      this.Add(cookie1, true);
    }

    /// <summary>
    /// Добавляет данные <see cref="T:System.Net.CookieCollection"/> в <see cref="T:System.Net.CookieContainer"/> для определенного URI.
    /// </summary>
    /// <param name="uri">URI объекта <see cref="T:System.Net.CookieCollection"/>, который должен добавляться в <see cref="T:System.Net.CookieContainer"/>. </param><param name="cookies">Объект <see cref="T:System.Net.CookieCollection"/>, добавляемый в <see cref="T:System.Net.CookieContainer"/>. </param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="cookies"/> имеет значение null. </exception><exception cref="T:System.ArgumentException">Домен для одного из файлов cookie в <paramref name="cookies"/> указан как null. </exception><exception cref="T:System.Net.CookieException">Один из файлов cookie в <paramref name="cookies"/> содержит недопустимый домен. </exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public void Add(Uri uri, CookieCollection cookies)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException("uri");
      if (cookies == null)
        throw new ArgumentNullException("cookies");
      bool isLocalDomain = this.IsLocalDomain(uri.Host);
      foreach (Cookie cookie1 in cookies)
      {
        Cookie cookie2 = cookie1.Clone();
        cookie2.VerifySetDefaults(cookie2.Variant, uri, isLocalDomain, this.m_fqdnMyDomain, true, true);
        this.Add(cookie2, true);
      }
    }

    /// <summary>
    /// Возвращает коллекцию <see cref="T:System.Net.CookieCollection"/>, содержащую экземпляры <see cref="T:System.Net.Cookie"/>, связанные с указанным URI.
    /// </summary>
    /// 
    /// <returns>
    /// Коллекция <see cref="T:System.Net.CookieCollection"/>, содержащая экземпляры <see cref="T:System.Net.Cookie"/>, связанные с указанным URI.
    /// </returns>
    /// <param name="uri">URI требуемых экземпляров <see cref="T:System.Net.Cookie"/>. </param><exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="uri"/> равно null. </exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public CookieCollection GetCookies(Uri uri)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException("uri");
      else
        return this.InternalGetCookies(uri);
    }

    /// <summary>
    /// Возвращает заголовок HTTP-cookie, содержащий файлы cookie HTTP, которые представляют экземпляры <see cref="T:System.Net.Cookie"/>, связанные с определенным URI.
    /// </summary>
    /// 
    /// <returns>
    /// Заголовок HTTP-cookie со строками, представляющими экземпляры <see cref="T:System.Net.Cookie"/>, разделяется точками с запятыми.
    /// </returns>
    /// <param name="uri">URI требуемых экземпляров <see cref="T:System.Net.Cookie"/>. </param><exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="uri"/> равно null. </exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public string GetCookieHeader(Uri uri)
    {
      if (uri == (Uri) null)
      {
        throw new ArgumentNullException("uri");
      }
      else
      {
        string optCookie2;
        return this.GetCookieHeader(uri, out optCookie2);
      }
    }

    /// <summary>
    /// Добавляет экземпляры <see cref="T:System.Net.Cookie"/> для одного или нескольких файлов cookie из заголовка HTTP-cookie в класс <see cref="T:System.Net.CookieContainer"/> для определенного URI.
    /// </summary>
    /// <param name="uri">URI для <see cref="T:System.Net.CookieCollection"/>. </param><param name="cookieHeader">Данные возвращаемого HTPP-сервером заголовка HTTP-cookie, содержащего экземпляры <see cref="T:System.Net.Cookie"/>, разделенные запятыми. </param><exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="uri"/> равно null. </exception><exception cref="T:System.ArgumentNullException">Параметр <paramref name="cookieHeader"/> имеет значение null. </exception><exception cref="T:System.Net.CookieException">Один из файлов cookie является недопустимым. -or- При добавлении одного из файлов cookie в контейнер произошла ошибка. </exception>
    public void SetCookies(Uri uri, string cookieHeader)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException("uri");
      if (cookieHeader == null)
        throw new ArgumentNullException("cookieHeader");
      this.CookieCutter(uri, (string) null, cookieHeader, true);
    }

    private void AddRemoveDomain(string key, PathList value)
    {
      lock (this)
      {
        if (value == null)
          this.m_domainTable.Remove((object) key);
        else
          this.m_domainTable[(object) key] = (object) value;
      }
    }

    internal void Add(Cookie cookie, bool throwOnError)
    {
      if (cookie.Value.Length > this.m_maxCookieSize)
      {
        if (!throwOnError)
          return;
        throw new CookieException(SR.GetString("net_cookie_size", (object) cookie.ToString(), (object) this.m_maxCookieSize));
      }
      else
      {
        try
        {
          PathList pathList = (PathList) this.m_domainTable[(object) cookie.DomainKey];
          if (pathList == null)
          {
            pathList = new PathList();
            this.AddRemoveDomain(cookie.DomainKey, pathList);
          }
          int cookiesCount = pathList.GetCookiesCount();
          CookieCollection cookieCollection = (CookieCollection) pathList[cookie.Path];
          if (cookieCollection == null)
          {
            cookieCollection = new CookieCollection();
            pathList[cookie.Path] = (object) cookieCollection;
          }
          if (cookie.Expired)
          {
            lock (cookieCollection)
            {
              int local_3 = cookieCollection.IndexOf(cookie);
              if (local_3 == -1)
                return;
              cookieCollection.RemoveAt(local_3);
              --this.m_count;
            }
          }
          else
          {
            if (cookiesCount >= this.m_maxCookiesPerDomain && !this.AgeCookies(cookie.DomainKey) || this.m_count >= this.m_maxCookies && !this.AgeCookies((string) null))
              return;
            lock (cookieCollection)
              this.m_count += cookieCollection.InternalAdd(cookie, true);
          }
        }
        catch (Exception ex)
        {
          if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
            throw;
          else if (throwOnError)
            throw new CookieException(SR.GetString("net_container_add_cookie"), ex);
        }
      }
    }

    private bool AgeCookies(string domain)
    {
      if (this.m_maxCookies == 0 || this.m_maxCookiesPerDomain == 0)
      {
        this.m_domainTable = new Hashtable();
        this.m_count = 0;
        return false;
      }
      else
      {
        int num1 = 0;
        DateTime dateTime1 = DateTime.MaxValue;
        CookieCollection cookieCollection1 = (CookieCollection) null;
        string str = (string) null;
        int index1 = 0;
        float num2 = 1f;
        if (this.m_count > this.m_maxCookies)
          num2 = (float) this.m_maxCookies / (float) this.m_count;
        foreach (DictionaryEntry dictionaryEntry in this.m_domainTable)
        {
          PathList pathList;
          if (domain == null)
          {
            str = (string) dictionaryEntry.Key;
            pathList = (PathList) dictionaryEntry.Value;
          }
          else
          {
            str = domain;
            pathList = (PathList) this.m_domainTable[(object) domain];
          }
          int num3 = 0;
          foreach (CookieCollection cc in (IEnumerable) pathList.Values)
          {
            index1 = this.ExpireCollection(cc);
            num1 += index1;
            this.m_count -= index1;
            num3 += cc.Count;
            DateTime dateTime2;
            if (cc.Count > 0 && (dateTime2 = cc.TimeStamp(CookieCollection.Stamp.Check)) < dateTime1)
            {
              cookieCollection1 = cc;
              dateTime1 = dateTime2;
            }
          }
          int num4 = Math.Min((int) ((double) num3 * (double) num2), Math.Min(this.m_maxCookiesPerDomain, this.m_maxCookies) - 1);
          if (num3 > num4)
          {
            Array instance1 = Array.CreateInstance(typeof (CookieCollection), pathList.Count);
            Array instance2 = Array.CreateInstance(typeof (DateTime), pathList.Count);
            foreach (CookieCollection cookieCollection2 in (IEnumerable) pathList.Values)
            {
              instance2.SetValue((object) cookieCollection2.TimeStamp(CookieCollection.Stamp.Check), index1);
              instance1.SetValue((object) cookieCollection2, index1);
              ++index1;
            }
            Array.Sort(instance2, instance1);
            index1 = 0;
            for (int index2 = 0; index2 < pathList.Count; ++index2)
            {
              CookieCollection cookieCollection2 = (CookieCollection) instance1.GetValue(index2);
              lock (cookieCollection2)
              {
                while (num3 > num4)
                {
                  if (cookieCollection2.Count > 0)
                  {
                    cookieCollection2.RemoveAt(0);
                    --num3;
                    --this.m_count;
                    ++num1;
                  }
                  else
                    break;
                }
              }
              if (num3 <= num4)
                break;
            }
            if (num3 > num4 && domain != null)
              return false;
          }
          if (domain != null)
            return true;
        }
        if (num1 != 0)
          return true;
        if (dateTime1 == DateTime.MaxValue)
          return false;
        lock (cookieCollection1)
        {
          for (; this.m_count >= this.m_maxCookies; --this.m_count)
          {
            if (cookieCollection1.Count > 0)
              cookieCollection1.RemoveAt(0);
            else
              break;
          }
        }
        return true;
      }
    }

    private int ExpireCollection(CookieCollection cc)
    {
      int count = cc.Count;
      int idx = count - 1;
      lock (cc)
      {
        for (; idx >= 0; --idx)
        {
          if (cc[idx].Expired)
            cc.RemoveAt(idx);
        }
      }
      return count - cc.Count;
    }

    internal bool IsLocalDomain(string host)
    {
      int indexB = host.IndexOf('.');
      if (indexB == -1 || host == "127.0.0.1" || (host == "::1" || host == "0:0:0:0:0:0:0:1") || string.Compare(this.m_fqdnMyDomain, 0, host, indexB, this.m_fqdnMyDomain.Length, StringComparison.OrdinalIgnoreCase) == 0)
        return true;
      string[] strArray = host.Split(new char[1]
      {
        '.'
      });
      if (strArray != null && strArray.Length == 4 && strArray[0] == "127")
      {
        int index;
        for (index = 1; index < 4; ++index)
        {
          switch (strArray[index].Length)
          {
            case 1:
              if ((int) strArray[index][0] >= 48 && (int) strArray[index][0] <= 57)
                continue;
              else
                goto label_10;
            case 2:
              if ((int) strArray[index][1] < 48 || (int) strArray[index][1] > 57)
                goto label_10;
              else
                goto case 1;
            case 3:
              if ((int) strArray[index][2] < 48 || (int) strArray[index][2] > 57)
                goto label_10;
              else
                goto case 2;
            default:
              goto label_10;
          }
        }
label_10:
        if (index == 4)
          return true;
      }
      return false;
    }

    internal CookieCollection CookieCutter(Uri uri, string headerName, string setCookieHeader, bool isThrow)
    {
      CookieCollection cookieCollection = new CookieCollection();
      CookieVariant variant = CookieVariant.Unknown;
      if (headerName == null)
      {
        variant = CookieVariant.Rfc2109;
      }
      else
      {
        for (int index = 0; index < CookieContainer.HeaderInfo.Length; ++index)
        {
          if (string.Compare(headerName, CookieContainer.HeaderInfo[index].Name, StringComparison.OrdinalIgnoreCase) == 0)
            variant = CookieContainer.HeaderInfo[index].Variant;
        }
      }
      bool isLocalDomain = this.IsLocalDomain(uri.Host);
      try
      {
        CookieParser cookieParser = new CookieParser(setCookieHeader);
        while (true)
        {
          Cookie cookie;
          do
          {
            cookie = cookieParser.Get();
            if (cookie != null)
            {
              if (ValidationHelper.IsBlankString(cookie.Name))
              {
                if (isThrow)
                  throw new CookieException(SR.GetString("net_cookie_format"));
              }
            }
            else
              goto label_19;
          }
          while (!cookie.VerifySetDefaults(variant, uri, isLocalDomain, this.m_fqdnMyDomain, true, isThrow));
          cookieCollection.InternalAdd(cookie, true);
        }
      }
      catch (Exception ex)
      {
        if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
          throw;
        else if (isThrow)
          throw new CookieException(SR.GetString("net_cookie_parse_header", new object[1]
          {
            (object) uri.AbsoluteUri
          }), ex);
      }
label_19:
      foreach (Cookie cookie in cookieCollection)
        this.Add(cookie, isThrow);
      return cookieCollection;
    }

    internal CookieCollection InternalGetCookies(Uri uri)
    {
      bool isSecure = uri.Scheme == Uri.UriSchemeHttps;
      int port = uri.Port;
      CookieCollection destination = new CookieCollection();
      ArrayList arrayList = new ArrayList();
      string host = uri.Host;
      int startIndex = host.IndexOf('.');
      int num1;
      if (startIndex == -1)
      {
        arrayList.Add((object) host);
        arrayList.Add((object) ("." + host));
        if (this.m_fqdnMyDomain != null && this.m_fqdnMyDomain.Length != 0)
        {
          arrayList.Add((object) (host + this.m_fqdnMyDomain));
          arrayList.Add((object) this.m_fqdnMyDomain);
          num1 = 3;
        }
        else
          num1 = 1;
      }
      else
      {
        arrayList.Add((object) host);
        arrayList.Add((object) ("." + host));
        arrayList.Add((object) host.Substring(startIndex));
        num1 = 2;
        if (host.Length > 2)
        {
          int num2 = host.LastIndexOf('.', host.Length - 2);
          if (num2 > 0)
            num2 = host.LastIndexOf('.', num2 - 1);
          if (num2 != -1)
          {
            while (startIndex < num2 && (startIndex = host.IndexOf('.', startIndex + 1)) != -1)
              arrayList.Add((object) host.Substring(startIndex));
          }
        }
      }
      foreach (string key in arrayList)
      {
        bool flag1 = false;
        bool flag2 = false;
        PathList pathList = (PathList) this.m_domainTable[(object) key];
        --num1;
        if (pathList != null)
        {
          foreach (DictionaryEntry dictionaryEntry in pathList)
          {
            string str = (string) dictionaryEntry.Key;
            if (uri.AbsolutePath.StartsWith(CookieParser.CheckQuoted(str)))
            {
              flag1 = true;
              CookieCollection source = (CookieCollection) dictionaryEntry.Value;
              source.TimeStamp(CookieCollection.Stamp.Set);
              this.MergeUpdateCollections(destination, source, port, isSecure, num1 < 0);
              if (str == "/")
                flag2 = true;
            }
            else if (flag1)
              break;
          }
          if (!flag2)
          {
            CookieCollection source = (CookieCollection) pathList["/"];
            if (source != null)
            {
              source.TimeStamp(CookieCollection.Stamp.Set);
              this.MergeUpdateCollections(destination, source, port, isSecure, num1 < 0);
            }
          }
          if (pathList.Count == 0)
            this.AddRemoveDomain(key, (PathList) null);
        }
      }
      return destination;
    }

    private void MergeUpdateCollections(CookieCollection destination, CookieCollection source, int port, bool isSecure, bool isPlainOnly)
    {
      lock (source)
      {
        for (int local_0 = 0; local_0 < source.Count; ++local_0)
        {
          bool local_1 = false;
          Cookie local_2 = source[local_0];
          if (local_2.Expired)
          {
            source.RemoveAt(local_0);
            --this.m_count;
            --local_0;
          }
          else
          {
            if (!isPlainOnly || local_2.Variant == CookieVariant.Plain)
            {
              if (local_2.PortList != null)
              {
                foreach (int item_0 in local_2.PortList)
                {
                  if (item_0 == port)
                  {
                    local_1 = true;
                    break;
                  }
                }
              }
              else
                local_1 = true;
            }
            if (local_2.Secure && !isSecure)
              local_1 = false;
            if (local_1)
              destination.InternalAdd(local_2, false);
          }
        }
      }
    }

    internal string GetCookieHeader(Uri uri, out string optCookie2)
    {
      CookieCollection cookies = this.InternalGetCookies(uri);
      string str1 = string.Empty;
      string str2 = string.Empty;
      foreach (Cookie cookie in cookies)
      {
        str1 = str1 + str2 + cookie.ToString();
        str2 = "; ";
      }
      optCookie2 = cookies.IsOtherVersionSeen ? "$Version=" + 1.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) : string.Empty;
      return str1;
    }
  }
}
