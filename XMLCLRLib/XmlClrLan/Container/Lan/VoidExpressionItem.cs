namespace XmlClrLan
{
    public class VoidExpressionItem : IExpressionItem<VOID>
    {
        public VOID Invoke(ModuleRunScope scope)
        {
            return VOID.Empty;
        }
    }

    public class VOID
    {
        /// <summary>
        /// 控制占位符（备用)
        /// </summary>
        public static readonly VOID Empty = new VOID();
    }

}
