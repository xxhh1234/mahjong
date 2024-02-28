namespace XH
{
    class CSharpSingleton<T> where T : new()
    {
        // 定义一个静态变量来保存类的实例
        private static T instance;
        public static T Instance
        { 
            get 
            {
                if (instance == null)
                {
                    if (instance == null)
                        instance = new T();
                }
                return instance;
            } 
         }
    }
}
