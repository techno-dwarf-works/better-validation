namespace Better.Validation.EditorAddons
{
    public class MutableTuple<T1, T2>
    {
        public MutableTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        
        public MutableTuple()
        {
            Item1 = default;
            Item2 = default;
        }

        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        
        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = Item1;
            item2 = Item2;
        }
    }
}