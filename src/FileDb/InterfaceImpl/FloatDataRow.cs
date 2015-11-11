namespace FileDb.InterfaceImpl
{
    class FloatDataRow : DataRow
    {
        public float Value
        {
            get { return (float) Values[0]; }
            set { Values[0] = value; }
        }
    }
}