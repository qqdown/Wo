using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wo.KancolleCore.KancolleNavigation
{
    class KancolleActionEdge : IEdge<KancolleSceneTypes>
    {
        private KancolleSceneTypes _source;
        private KancolleSceneTypes _target;
        public KancolleAction Action { get; set; }

        public KancolleSceneTypes Source
        {
            get
            {
                return _source;
            }
        }

        public KancolleSceneTypes Target
        {
            get
            {
                return _target;
            }
        }

        public KancolleActionEdge(KancolleSceneTypes from, KancolleSceneTypes to, KancolleAction action)
        {
            this._source = from;
            this._target = to;
            this.Action = action;
        }
    }
}
