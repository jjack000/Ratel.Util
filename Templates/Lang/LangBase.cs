using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewIP
{
    public class LangBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected bool kr;
        public void Korean()
        {
            kr = true;
            Update();
        }

        public void English()
        {
            kr = false;
            Update();
        }

        public void Update()
        {
            // 모든 property를 업데이트
            for (int i = 0; i < GetType().GetProperties().Length; i++)
            {
                OnPropertyChanged(GetType().GetProperties()[i].Name);
            }
        }
    }
}
