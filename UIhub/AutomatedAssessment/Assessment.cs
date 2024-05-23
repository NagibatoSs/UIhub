namespace UIhub.AutomatedAssessment
{
    public abstract class Assessment
    {
        public double Rate
        {
            get
            { return _rate; }
            protected set
            {
                _rate = value;
                if (value < 0)
                    _rate = 0;
                if (value > 10)
                    _rate = 10;
            }
        }
        private double _rate;
        public string RateMessage { get; protected set; }
        protected string xamlText;

        public abstract Tuple<double,string> DoAssessment(string xamlText);

    }
}
