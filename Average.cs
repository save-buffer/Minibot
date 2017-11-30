using System;
using Scarlet;
using Scarlet.Utilities;

namespace UseRobot
{
    /// <summary>
    /// The Average filter is intended
    /// for use as an average-gathering
    /// system, using a rolling average
    /// with "roll-length" <c>FilterCount</c>.</summary>
    /// 
    /// Implementation Details:
    /// 
    /// *Construct Average filter given a rolling
    ///  filter length, <c>FilterCount</c>
    /// 
    /// *Iteratively add values into the filter
    ///  using <c>Feed(T Input)</c>
    /// 
    /// *Get the filter output by calling
    ///  <c>YourFilterInstance.Output</c>
    /// 
    /// <typeparam name="T">
    /// A type, which must be a numeric.</typeparam>
    public class Average<T> where T : struct, IComparable, IComparable<long>, IConvertible, IEquatable<long>, IFormattable
    {

        private T Output; // Filter output
        private T[] AverageArray; // Stored average array
        private T CurSum;         // Current sum of the average array 
        private int FilterCount,        // Size of the average array
                    Index,              // Current index of the average array
                    Iterations;         // Number of iterations in the filter

        public static bool IsNumericType(Type Type)
        {
            switch (Type.GetTypeCode(Type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Construct an average filter with
        /// given roll-length.
        /// </summary>
        /// <param name="FilterCount">
        /// Roll length for the average filter.</param>
        public Average(int FilterCount = 10)
        {
            if (!IsNumericType(typeof(T)))
            {
                Log.Output(Log.Severity.ERROR, Log.Source.OTHER, "Average filter cannot be instantiated with non-numeric type.");
                throw new ArgumentException("Cannot create filter of non-numeric type: " + typeof(T).ToString());
            } // We can now assert that T is a numeric type                      
            this.Output = default(T);
            this.CurSum = default(T);
            this.Index = 0;
            this.Iterations = 0;
            this.FilterCount = FilterCount;
            this.AverageArray = new T[this.FilterCount];
            this.InitializeArray(); // Initialize average array to defaults
        }

        /// <summary>
        /// Feeds a value into the filter.
        /// </summary>
        /// <param name="Input">
        /// Value to feed into the filter.</param>
        public void Feed(T Input)
        {
            // Increase number of iterations by 1
            this.Iterations++;
            // Store input as a dynamic type since we know T is a numeric
            //          dynamic dynamicInput = Input;
            // Subtract current array index value from sum
            this.CurSum -= this.AverageArray[this.Index];
            // Add current value to sum
            this.CurSum += Input;
            // Store curent value in old spot
            this.AverageArray[this.Index] = Input;
            // Increment index. Go back to zero if index + 1 == filterCount
            this.Index = (this.Index + 1) % this.FilterCount; // Increment index. Go back to zero if index + 1 == filterCount
            // Divide output by either number of iterations or filter length
            this.Output = this.CurSum / (Math.Min(this.Iterations, this.FilterCount));
        }

        /// <summary>
        /// Feeds filter with specified rate.
        /// Not used for average filter.
        /// </summary>
        /// <param name="Input">
        /// Value to feed into the filer.</param>
        /// <param name="Rate">
        /// Current rate to feed into the filter.</param>
        public void Feed(T Input, T Rate)
        {
            this.Feed(Input); // Average filter is independent of rate
        }

        /// <summary>
        /// Initializes dynamic number array to  
        /// all zeros.</summary>
        private void InitializeArray()
        {
            for (int i = 0; i < this.AverageArray.Length; i++)
            {
                this.AverageArray[i] = default(T);
            }
        }

        public T GetOutput() { return Output; }
    }
}