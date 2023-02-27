namespace NeuroWeb.EXMPL.OBJECTS {
    public class ConvolutionLayer {

        public ConvolutionLayer(Filter filter, Configuration configuration) {
            Filter        = filter;
            Configuration = configuration;
        }
        
        public Filter Filter { get; set; }
        public Configuration Configuration { get; set; }
    }
}