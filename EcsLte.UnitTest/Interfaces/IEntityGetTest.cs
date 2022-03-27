using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.Interfaces
{
	internal interface IEntityGetTest
	{
		void GetEntities();
		void GetEntities_Destroy();
		void HasEntity();
		void HasEntity_Destroy();
	}
}
