using System;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Interfaces;

namespace code.Grid.Models
{

	public class SkyGridControlValueBase : IGridControlValue
	{

		public GridControl Control
		{
			get;
		}

		public virtual bool IsValid
		{
			get
			{
				return true;
			}
		}

		public virtual string GetSearchableText()
		{
			return String.Empty;
		}

		public SkyGridControlValueBase(GridControl control)
		{
			Control = control;
		}

	}

}