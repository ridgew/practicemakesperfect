namespace XmlClrLan
{
    /// <summary>
    /// 表达式或运算项
    /// </summary>
    public interface IExpressionItem<Target>
    {
        Target Invoke(ModuleRunScope scope);
    }

}
