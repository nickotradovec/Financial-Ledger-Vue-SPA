import CounterExample from 'components/counter-example'
import FetchData from 'components/fetch-data'
import HomePage from 'components/home-page'
import AccountList from 'components/account-list'
import JournalList from 'components/journal-entries'

export const routes = [
  { name: 'home', path: '/', component: HomePage, display: 'Home', icon: 'home' },
  { name: 'counter', path: '/counter', component: CounterExample, display: 'Counter', icon: 'graduation-cap' },
  { name: 'fetch-data', path: '/fetch-data', component: FetchData, display: 'Fetch data', icon: 'list' },
  { name: 'accountList', path: '/account-list', component: AccountList, display: 'Account Maintenance', icon: 'list' },
  { name: 'journalEntries', path: '/journal-entries', component: JournalList, display: 'Journal Entries', icon: 'list' }
]
