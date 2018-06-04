using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Collections.ObjectModel;

namespace ViewModel
{
    public class MainViewModel
    {
        Spiel wuerfelspiel = null;
        public MainViewModel()
        {
            wuerfelspiel = new Spiel();
            wuerfelspiel.LoadSpieler();
        }
        public ObservableCollection<Spieler> LSTSpieler
        {
            get { return wuerfelspiel.LstSpieler; }
        }
     
    }
}
