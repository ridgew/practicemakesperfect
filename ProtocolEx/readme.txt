http://www.cnblogs.com/dsky/archive/2013/03/28/2986727.html
https://msdn.microsoft.com/zh-cn/library/ee318402(v=vs.85).aspx

异步可插协议处理器可以被指定为在所有进程内有效，而异步可插命名空间协议处理器仅被指定为在某个具体的进程内有效。

 

浏览器扩展系列————异步可插入协议（pluggable protocol）的实现
      IE中有很多我们比较熟悉的协议，如http,https,mailto,ftp等。当然你也可以实现自己定义的协议，稍微谈一下这里所说的协议，从我的理解来说这里的协议只有当你的网页引用某个资源时才会调用，而不是随便在某个属性的值前面加上某个协议的名称就可以了。常见的协议调用如img的src属性中，很多元素style中的background-image属性中，还有a标签的href属性中。

       言归正传，前面说到的实现自定义协议就用到了一种IE下异步可插入协议的技术。

       从分类上来说，这种异步可插入协议的技术还分为两种：

 

永久的异步可插入协议，就像http，https，mailto这种不论在ie中或是其它用到浏览器控件中使用。
临时的异步可插入协议，只能用在某个进程内，用完可以擦除。
 

     更详细介绍异步可插入协议的资源有http://www.cppblog.com/bigsml/archive/2008/03/23/45145.html。

     因为网上介绍永久的异步可插入协议的资源还很多，如codeproject上的：

http://www.cppblog.com/bigsml/archive/2008/03/23/45145.html
http://www.codeproject.com/KB/aspnet/AspxProtocol.aspx
    这篇就主要谈谈如何实现临时的异步可插入协议的方法。



此外，在构造IInternetSession的时候还用到了一个外部方法：

[DllImport("urlmon.dll")]

private static extern void CoInternetGetSession(int sessionMode,

out IInternetSession session, int reserved);

 

预备的知识介绍完，下面就是具体实现了。

一般方法是在一个类中实现IInternetProtocol，IInternetProtocolRoot，IInternetProtocolInfo三个接口，然后通过IInternetSession接口的RegisterNameSpace方法来注册这个自定义协议，用完这后再调用UnregisterNameSpace方法来注销这个自定义协议。

关于IE和IInternetProtocol，IInternetProtocolRoot，IInternetProtocolInfo三个接口的调用流程可以参考msdn上的介绍,中文版的翻译可以参考：

http://www.cnblogs.com/volnet/archive/2008/03/28/About_Asynchronous_Pluggable_Protocols.html
