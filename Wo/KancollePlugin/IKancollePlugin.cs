using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wo.KancolleCore;
using Wo.ViewModels;

namespace Wo.KancollePlugin
{
    public interface IKancollePlugin
    {
        int Version { get; }

        string Name { get; }

        string Description { get; }

        UserControl PluginPanel { get; }

        bool NewWindow { get; }

        void OnInit(GeneralViewModel generalViewModel);

        void OnGameStart(GeneralViewModel generalViewModel, KancolleGameData gameData);

        void OnGameDataUpdated(GeneralViewModel generalViewModel, KancolleCore.KancolleGameData gameData);
    }
}
