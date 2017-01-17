namespace Edubase.Common
{
    public struct Returns<T>
    {
        public T Object { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success => ErrorMessage == null;
        public Returns<T> Set(T obj) { Object = obj; return this; }
    }
}
