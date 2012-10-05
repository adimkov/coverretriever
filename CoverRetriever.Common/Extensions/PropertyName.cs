namespace CoverRetriever.Common.Extensions
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///  Helper methods for get property name
    /// </summary>
    public static class PropertyName
    {
        /// <summary>
        /// For the specified expression.
        /// </summary>
        /// <typeparam name="T">Type of instance for looking in.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Name of property.</returns>
        public static string For<T>(Expression<Func<T, object>> expression)
        {
            Expression body = expression.Body;
            return GetMemberName(body);
        }

        /// <summary>
        /// For the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Name of property.</returns>
        public static string For(Expression<Func<object>> expression)
        {
            Expression body = expression.Body;
            return GetMemberName(body);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Name of property.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static string GetMemberName(Expression expression)
        {
            if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression)expression;

                if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    return GetMemberName(memberExpression.Expression) + "." + memberExpression.Member.Name;
                }
                return memberExpression.Member.Name;
            }

            if (expression is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)expression;

                if (unaryExpression.NodeType != ExpressionType.Convert)
                    throw new ArgumentException(string.Format("Cannot interpret member from {0}", expression));

                return GetMemberName(unaryExpression.Operand);
            }

            throw new ArgumentException(string.Format("Could not determine member from {0}", expression));
        }
    }
}