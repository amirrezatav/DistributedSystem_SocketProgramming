using Client.Persistance;
using MaterialDesignThemes.Wpf;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client.ViewModel
{

    public class PersonViewModel : Observable
    {
        private readonly long _id;
        public ObservableCollection<Person> Items { get; set; } = new ObservableCollection<Person>();
        public Person Sellected { get; set; } = new Person();
        
        public void Add(Person item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Items.Insert(0, item);
            });
        }
        public void Clear()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
            Items.Clear();
            });
        }
        public void AddRange(List<Person> items)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in items)
                {
                    Items.Insert(0, item);
                }
            });
            
        }
        public void Delete(Person item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Items.Remove(item);
            });
        }
    }

    public class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual bool SetProperty<T>(ref T storage, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, newValue)) return false;
            storage = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected virtual bool SetProperty<T>(T storage, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, newValue)) return false;
            storage = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected virtual bool SetProperty<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
