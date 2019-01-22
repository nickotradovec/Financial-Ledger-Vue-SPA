import moment from './moment'

let formatting = {

    methods: {

        dataDate: function(dt) {
            return moment(dt).format('YYYY-MM-DD');
        },

        formatDate: function(date) {
              
            var mDate = moment.utc(date);

            if ( mDate.diff('9999-12-31', 'days') >= 0 ) {
                return ""
            }
            return mDate.format('DD-MMM-YY');
        },

        isActive: function(commence, cease) {

            var mCommence = moment.utc(commence);
            var mCease = moment.utc(cease);
            var mNow = moment.now();
            
            if ( mNow.diff(mCommence, 'days') < 0 ) {
                return false
            }
            if ( mNow.diff(mCease, 'days') >= 0 ) {
                return false
            }
            return true;
        },

        isFuture: function(dt) {
            return moment(new moment(dt)).isAfter(moment.now());
        },

        validateDate: function(dt) {
            return true
        }
    }
}

export default formatting