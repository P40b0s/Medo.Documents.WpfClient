﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvallonEditTest
{
    class BinarySorter<TKey>
    {
        #region Public Methods

        /// <summary>
        /// Optional comparer used for sorting keys
        /// </summary>
        private IComparer<TKey> _comparer;

        #endregion Private Fields

        // ************************************************************************
        // Public Methods
        // ************************************************************************
        #region Public Methods

        /// <summary>
        /// Constructor that takes a comparer
        /// </summary>
        /// <param name="comparer"></param>
        public BinarySorter(IComparer<TKey> comparer = null)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Gets the position for a key to be inserted such that the sort order is maintained.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInsertIndex(int count, TKey key, Func<int, TKey> indexToKey)
        {
            return BinarySearchForIndex(0, count - 1, key, indexToKey);
        }

        #endregion Public Methods

        // ************************************************************************
        // Private Methods
        // ************************************************************************
        #region Private Methods

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        private int Compare(TKey key1, TKey key2)
        {
            // First use the comparer if it is set
            if (_comparer != null)
            {
                return _comparer.Compare(key1, key2);
            }

            // Then try using the default Collections comparer, only try this once, then skip it following tries
            if (!_defaultCompareFailed)
            {
                try
                {
                    return System.Collections.Comparer.Default.Compare(key1, key2);
                }
                catch (Exception)
                {
                    _defaultCompareFailed = true;
                }
            }

            // Last resort do a string compare
            return string.Compare(key1.ToString(), key2.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
        private bool _defaultCompareFailed = false;

        /// <summary>
        /// Searches for the index of the insertion point for the key passed in such that
        /// the sort order is maintained. Implemented as a non-recursive method.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private int BinarySearchForIndex(int low, int high, TKey key, Func<int, TKey> indexToKey)
        {
            while (high >= low)
            {

                // Calculate the mid point and determine if the key passed in
                // should be inserted at this point, below it, or above it.
                int mid = low + ((high - low) >> 1);
                int result = Compare(indexToKey(mid), key);

                // Return the current position, or change the search bounds
                // to be above or below the mid point depending on the result.
                if (result == 0)
                    return mid;
                else if (result < 0)
                    low = mid + 1;
                else
                    high = mid - 1;
            }
            return low;
        }

        #endregion Private Methods
    }
}
