using ESRI.ArcGIS.Geodatabase;

namespace LoowooTech.Stock.ArcGISTool
{
    public class GISField
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int Length { get; set; }
        public esriFieldType Type { get; set; }
    }
}
