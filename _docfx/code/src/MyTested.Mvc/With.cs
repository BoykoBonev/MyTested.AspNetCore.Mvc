﻿namespace MyTested.Mvc
{
    /// <summary>
    /// Provides easy replacing of expression method argument values.
    /// </summary>
    public static class With
    {
        /// <summary>
        /// Indicates that a argument should not be considered in method call lambda expression.
        /// </summary>
        /// <typeparam name="TParameter">Type of parameter.</typeparam>
        /// <returns>Default value of the parameter.</returns>
        public static TParameter No<TParameter>()
        {
            return default(TParameter);
        }

        /// <summary>
        /// Indicates that a argument should not be considered in method call lambda expression.
        /// </summary>
        /// <typeparam name="TParameter">Type of parameter.</typeparam>
        /// <returns>Default value of the parameter.</returns>
        public static TParameter Any<TParameter>()
        {
            return default(TParameter);
        }
    }
}
