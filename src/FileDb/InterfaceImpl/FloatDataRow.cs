namespace FileDb.InterfaceImpl
{
    public class FloatDataRow : DataRow
    {
        public float Value
        {
            get { return (float) Values[0]; }
            set { Values = new object[] { value}; }
        }
    }
}