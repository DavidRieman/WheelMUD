namespace NCalc.Domain
{
    public class BinaryExpression : LogicalExpression
    {
        public BinaryExpression(BinaryExpressionType type, LogicalExpression leftExpression, LogicalExpression rightExpression)
        {
            this.type = type;
            this.LeftExpression = leftExpression;
            this.RightExpression = rightExpression;
        }

        public LogicalExpression LeftExpression { get; set; }

        public LogicalExpression RightExpression { get; set; }

        private BinaryExpressionType type;
        
        public BinaryExpressionType Type
        {
            get { return type; }
            set { type = value; }
        }

        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public enum BinaryExpressionType
    {
        And,
        Or,
        NotEqual,
        LesserOrEqual,
        GreaterOrEqual,
        Lesser,
        Greater,
        Equal,
        Minus,
        Plus,
        Modulo,
        Div,
        Times,
        BitwiseOr,
        BitwiseAnd,
        BitwiseXOr,
        LeftShift,
        RightShift,
        Unknown
    }
}
