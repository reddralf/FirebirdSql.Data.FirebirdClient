/*
 *	Firebird ADO.NET Data provider for .NET and Mono 
 * 
 *	   The contents of this file are subject to the Initial 
 *	   Developer's Public License Version 1.0 (the "License"); 
 *	   you may not use this file except in compliance with the 
 *	   License. You may obtain a copy of the License at 
 *	   http://www.firebirdsql.org/index.php?op=doc&id=idpl
 *
 *	   Software distributed under the License is distributed on 
 *	   an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either 
 *	   express or implied. See the License for the specific 
 *	   language governing rights and limitations under the License.
 * 
 *	Copyright (c) 2002, 2006 Carlos Guzman Alvarez
 *	All Rights Reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;

using FirebirdSql.Data.Common;

namespace FirebirdSql.Data.FirebirdClient
{
	/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/overview/*'/>
#if	(NET)
	[ListBindable(false)]
#endif
	public sealed class FbParameterCollection : DbParameterCollection
	{
		#region � Fields �

		private List<FbParameter> parameters;

		#endregion

		#region � Indexers �

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/indexer[@name="Item(System.String)"]/*'/>
#if	(!NETCF)
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public new FbParameter this[string parameterName]
		{
			get { return this[this.IndexOf(parameterName)]; }
			set { this[this.IndexOf(parameterName)] = value; }
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/indexer[@name="Item(System.Int32)"]/*'/>
#if	(!NETCF)
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public new FbParameter this[int index]
		{
			get { return this.parameters[index]; }
			set { this.parameters[index] = value; }
		}

		#endregion

		#region � DbParameterCollection overriden properties �

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/property[@name="Count"]/*'/>
#if	(!NETCF)
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public override int Count
		{
			get { return this.parameters.Count; }
		}

		public override bool IsFixedSize
		{
			get { return ((IList)this.parameters).IsFixedSize; }
		}

		public override bool IsReadOnly
		{
			get { return ((IList)this.parameters).IsReadOnly; }
		}

		public override bool IsSynchronized
		{
			get { return ((ICollection)this.parameters).IsSynchronized; }
		}

		public override object SyncRoot
		{
			get { return ((ICollection)this.parameters).SyncRoot; }
		}

		#endregion

		#region � Constructors �

		internal FbParameterCollection()
		{
            this.parameters = new List<FbParameter>();
		}

		#endregion

		#region � DbParameterCollection overriden methods �

        public void AddRange(FbParameter[] values)
        {
            this.AddRange(values);
        }

		public override void AddRange(Array values)
		{
			foreach (FbParameter p in values)
			{
				this.Add(p);
			}
		}

		public FbParameter AddWithValue(string parameterName, object value)
		{
			return this.Add(new FbParameter(parameterName, value));
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Add(System.String,System.Object)"]/*'/>
		public FbParameter Add(string parameterName, object value)
		{
			return this.Add(new FbParameter(parameterName, value));
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Add(System.String,FbDbType)"]/*'/>
		public FbParameter Add(string parameterName, FbDbType type)
		{
			return this.Add(new FbParameter(parameterName, type));
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Add(System.String,FbDbType,System.Int32)"]/*'/>
		public FbParameter Add(string parameterName, FbDbType fbType, int size)
		{
			return this.Add(new FbParameter(parameterName, fbType, size));
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Add(System.String,FbDbType,System.Int32,System.String)"]/*'/>
		public FbParameter Add(string parameterName, FbDbType fbType, int size, string sourceColumn)
		{
			return this.Add(new FbParameter(parameterName, fbType, size, sourceColumn));
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Add(FbParameter)"]/*'/>
		public FbParameter Add(FbParameter value)
		{
			lock (this.SyncRoot)
			{
				if (value == null)
				{
					throw new ArgumentException("The value parameter is null.");
				}
				if (value.Parent != null)
				{
					throw new ArgumentException("The FbParameter specified in the value parameter is already added to this or another FbParameterCollection.");
				}
				if (value.ParameterName == null ||
					value.ParameterName.Length == 0)
				{
					value.ParameterName = this.GenerateParameterName();
				}
				else
				{
					if (this.IndexOf(value) != -1)
					{
						throw new ArgumentException("FbParameterCollection already contains FbParameter with ParameterName '" + value.ParameterName + "'.");
					}
				}

				this.parameters.Add(value);

				return value;
			}
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Add(System.Object)"]/*'/>
		public override int Add(object value)
		{
			if (!(value is FbParameter))
			{
				throw new InvalidCastException("The parameter passed was not a FbParameter.");
			}

			return this.IndexOf(this.Add(value as FbParameter));
		}

        public bool Contains(FbParameter value)
        {
            return this.parameters.Contains(value);
        }

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Contains(System.Object)"]/*'/>
		public override bool Contains(object value)
		{
			return this.parameters.Contains((FbParameter)value);
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Contains(System.String)"]/*'/>
		public override bool Contains(string parameterName)
		{
			return (-1 != this.IndexOf(parameterName));
		}

        public int IndexOf(FbParameter value)
        {
            return this.parameters.IndexOf(value);
        }

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="IndexOf(System.Object)"]/*'/>
		public override int IndexOf(object value)
		{
			return this.parameters.IndexOf((FbParameter)value);
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="IndexOf(System.String)"]/*'/>
		public override int IndexOf(string parameterName)
		{
			int index = 0;
			foreach (FbParameter item in this.parameters)
			{
				if (GlobalizationHelper.CultureAwareCompare(item.ParameterName, parameterName))
				{
					return index;
				}
				index++;
			}
			return -1;
		}

        public void Insert(int index, FbParameter value)
        {
            this.parameters.Insert(index, value);
        }

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Insert(System.Int32,System.Object)"]/*'/>
		public override void Insert(int index, object value)
		{
			this.parameters.Insert(index, (FbParameter)value);
		}
		
		public void Remove(FbParameter value)
		{
            if (!(value is FbParameter))
            {
                throw new InvalidCastException("The parameter passed was not a FbParameter.");
            }
            if (!this.Contains(value))
            {
                throw new SystemException("The parameter does not exist in the collection.");
            }

            this.parameters.Remove(value);

            ((FbParameter)value).Parent = null;
        }

        /// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Remove(System.Object)"]/*'/>
        public override void Remove(object value)
        {
            if (!(value is FbParameter))
            {
                throw new InvalidCastException("The parameter passed was not a FbParameter.");
            }
            if (!this.Contains(value))
            {
                throw new SystemException("The parameter does not exist in the collection.");
            }

            this.parameters.Remove((FbParameter)value);

            ((FbParameter)value).Parent = null;
        }

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="RemoveAt(System.Int32)"]/*'/>
		public override void RemoveAt(int index)
		{
			if (index < 0 || index > this.Count)
			{
				throw new IndexOutOfRangeException("The specified index does not exist.");
			}

			FbParameter parameter = this[index];
			this.parameters.RemoveAt(index);
			parameter.Parent = null;
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="RemoveAt(System.String)"]/*'/>
		public override void RemoveAt(string parameterName)
		{
			this.RemoveAt(this.IndexOf(parameterName));
		}

        public void CopyTo(FbParameter[] array, int index)
        {
            this.parameters.CopyTo(array, index);
        }

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="CopyTo(System.Array,System.Int32)"]/*'/>
		public override void CopyTo(Array array, int index)
		{
			((IList)this.parameters).CopyTo(array, index);
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="Clear"]/*'/>
		public override void Clear()
		{
			this.parameters.Clear();
		}

		/// <include file='Doc/en_EN/FbParameterCollection.xml'	path='doc/class[@name="FbParameterCollection"]/method[@name="GetEnumerator"]/*'/>
		public override IEnumerator GetEnumerator()
		{
			return this.parameters.GetEnumerator();
		}
		
		#endregion

		#region � DbParameterCollection overriden protected methods �

		protected override DbParameter GetParameter(string parameterName)
		{
			return this[parameterName];
		}

		protected override DbParameter GetParameter(int index)
		{
			return this[index];
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			this[index] = (FbParameter)value;
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			this[parameterName] = (FbParameter)value;
		}

		#endregion

		#region � Private Methods �

		private string GenerateParameterName()
		{
			int index = this.Count + 1;
			string name = String.Empty;

			while (index > 0)
			{
				name = "Parameter" + index.ToString(CultureInfo.InvariantCulture);

				if (this.IndexOf(name) == -1)
				{
					index = -1;
				}
				else
				{
					index++;
				}
			}

			return name;
		}

		#endregion
	}
}