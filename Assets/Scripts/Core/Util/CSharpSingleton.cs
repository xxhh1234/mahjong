using Unity.VisualScripting;

namespace XH
{
    class CSharpSingleton<T> where T : new()
    {
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        // 定义一个静态变量来保存类的实例
        private static T instance;
        public static T Instance
        { 
            get 
            {
                // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
                // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
                // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
                // 双重锁定只需要一句判断就可以了
                if (instance == null)
                {
                    lock (locker)
                    {
                        // 如果类的实例不存在则创建，否则直接返回
                        if (instance == null)
                        {
                            instance = new T();
                        }
                    }
                }
                return instance;
            } 
         }
    }
}
