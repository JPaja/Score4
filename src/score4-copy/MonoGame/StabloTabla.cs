namespace Score4AI
{

    public class StabloTabla
    {
        public StabloTabla(Tabla tabla, bool jeList = true) //inicijalizacija je dodata zbog optimizacije 
        {
            Tabla = tabla;
            Potezi = jeList ? new StabloTabla[16] : null;
        }

        public sbyte? Vrednost;
        public Tabla Tabla;
        public StabloTabla[] Potezi;
    }
}
