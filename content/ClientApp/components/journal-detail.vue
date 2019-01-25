<script>
import formatting from "../libraries/formatting";
//import FontAwesomeIcon from '../icons.js'

export default {
  name: "journalmodal",

  mixins: [formatting],

  computed: {
    modalVisible: function() {
      return this.mode > 0;
    },
    isReadOnly: function() {
      return this.mode <= 1;
    },
    accountsNV: function() {
      var actsNV = [];
      //var i;
      for (var i = 0; i < this.accounts.length; i += 1) {
        actsNV.push({
          value: this.accounts[i].accountId,
          text: this.accounts[i].accountName
        });
      }
      return actsNV;
    }
  },

  data() {
    return {
      mode: 0,
      journalId: -1,
      journalName: null,
      journalDate: null,
      journalAmount: 0,
      comment: null,
      debits: [],
      credits: [],
      validationErrors: [],
      accounts: [],
      deletetest: ""
    };
  },

  methods: {
    // Click events
    close() {
      console.log("Close event raised.");
      this.reset(); },
    change() {
      console.log("change event raised.");
      this.mode = 2; },
    cancel() { 
      if ( this.journalId >= 0 ) { this.getJournal(this.journalId, false); }
      else { this.reset(); } },
    modalAddJournal() {
      this.mode = 3; },

    save() {
      console.log(`Save event raised from mode: ${this.mode.toString()}`);
      if (!this.validate()) {
        return;
      }
      if (this.journalId < 0) {
        console.log("Add new journal");
        this.saveJournal(`/api/JournalMaintenance/AddNewJournal`);
      } else {
        console.log(`Save changes to journal ${this.journalId}`);
        this.saveJournal(`/api/JournalMaintenance/UpdateJournal`);
      }
    },

    // Internal events
    reset() {
      console.log("reset called");
      this.mode = 0;
      this.journalId = -1;
      this.journalName = null;
      this.journalDate = null;
      this.journalAmount = 0;
      this.comment = null;
      this.debits = [];
      this.credits = [];
      this.validationErrors = [];
    },

    async getJournal(id, editmode) {
      if (id < 0) {
        console.log("ID not specified for getting journal.");
        return;
      }
      this.journalId = id;

      try {
        console.log("Try retrive data for journal: " & this.journalId);
        let response = await this.$http.get(
          `/api/JournalMaintenance/GetJournalDetails?id=${this.journalId}`
        );
        console.log(response);
        this.journalId = response.data.journalId;
        this.journalName = response.data.journalName;
        this.journalDate = this.dataDate(response.data.journalDate);
        this.journalAmount = response.data.journalAmount;
        this.comment = response.data.comment;
        this.credits = [];
        this.debits = [];

        for (var i = 0; i < response.data.details.length; i++) {
          if (response.data.details[i].debitCredit == "D") {
            this.debits.push({
              accountId: response.data.details[i].account.accountId,
              accountName: response.data.details[i].account.accountName,
              amount: -1 * response.data.details[i].amount
            });
          } else {
            this.credits.push({
              accountId: response.data.details[i].account.accountId,
              accountName: response.data.details[i].account.accountName,
              amount: response.data.details[i].amount
            });
          }
        }
        console.log(this);

        if (editmode) {
          this.mode = 2;
        } else {
          this.mode = 1;
        }
      } catch (err) {
        window.alert("Unable to view the journal at this time");
        this.reset();
        console.log(err);
      }
    },

    async saveJournal(api) {
      var comp = this;
      let dta = {
        journalId: this.journalId,
        journalName: this.journalName,
        journalDate: this.journalDate,
        journalAmount: this.journalAmount,
        comment: this.comment
      };

      try {
        let response = await comp.$http.post(api, dta);
        comp.reset();
        comp.$emit("closeModalWithRefresh");
      } catch (err) {
        console.log(err);
        alert("unable to save journal at this time.");
      }
    },

    async loadAccounts() {
      try {
        let response = await this.$http.get(`/api/AccountList/AccountsList`);
        console.log(response);
        this.accounts = response.data;
      } catch (err) {
        window.alert(err);
        console.log(err);
      }
    },

    addCredit() {
      this.credits.push({ accountId: 0, amount: 0 });
    },
    deleteCredit(index) {
      this.credits.splice(index, 1);
    },
    addDebit() {
      this.debits.push({ accountId: 0, amount: 0 });
    },
    deleteDebit(index) {
      this.debits.splice(index, 1);
    },

    validate() {
      return false;
    }
  },

  async created() {
    // accounts could be retrieved earlier and passed in as a prop, but could change
    // for this reason, done here for robustness
    await this.loadAccounts();
  }
};
</script>

<template>
  <span v-if="modalVisible">
    <transition name="modal-fade">
      <div class="modal-backdrop">
        <div
          class="modaldetailjournal"
          role="dialog"
          aria-labelledby="modalTitle"
          aria-describedby="modalDescription"
        >
          <header class="modal-header" id="modalTitle">
            <h3 v-if="mode==3">Add New Journal</h3>
            <h3 v-else>Journal Details</h3>
          </header>

          <section class="modal-body" id="modalDescription">
            <div v-if="validationErrors.length>0" class="validationErrors" id="validationErrors">
              <ul
                v-for="error in validationErrors"
                :key="error"
                style="margin-left:0;padding-left:0"
              >
                <li
                  style="list-style: none; padding-left:0px; color: maroon; line-height:1"
                >{{error.errorText}}</li>
              </ul>
            </div>

            <div class="inputElement">
                <label for="journalId">Journal Id</label>
                <input
                  type="number"
                  v-model="journalId"
                  placeholder="Journal Id"
                  id="journalId"
                  disabled="true"
                >
            </div>

            <div class="inputElement">
              <label for="journalName">Journal Name</label>
              <input
                type="text"
                v-model="journalName"
                placeholder="Journal Name"
                :disabled="isReadOnly"
                id="journalName"
              >
            </div>

            <div class="inputElement">
              <label for="journalDate">Journal Date</label>
              <input
                type="date"
                @input="dataDate(journalDate)"
                v-model="journalDate"
                :disabled="isReadOnly"
                id="journalDate"
              >
            </div>

            <div class="inputElement">
              <label for="comment">Comment</label>
              <input
                type="text"
                v-model="comment"
                placeholder="Comment"
                :disabled="isReadOnly"
                id="comment"
              >
            </div>
          </section>

          <div class="dcrow">
            <div class="column" id="debits">
              <div class="header">
                <h4>Debits</h4>
                <button v-if="!isReadOnly" v-on:click="addDebit" style="margin-right:10px">Add</button>
              </div>
              <table class="table">
                <thead>
                  <tr>
                    <th v-if="!isReadOnly">  </th>
                    <th>Account Name</th>
                    <th class="tableamount">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(debit, index) in debits" :key="debit.accountId" style="line-height:1; margin:0">
                    <td v-if="!isReadOnly"><icon icon="trash" v-on:click="deleteDebit(index)" style="margin-top:5px !important;"/></td>
                    <b-form-select
                      v-model="debit.accountId"
                      :options="accountsNV"
                      class="mb-3"
                      size="sm"
                      style="margin-top:10px"
                      :disabled="isReadOnly"
                    />
                    <td><input type="number" class="inputAmount" v-model="debit.amount" :disabled="isReadOnly"/></td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="column" id="credits">
              <div class="header">
                <h4>Credits</h4>
                <button v-if="!isReadOnly" v-on:click="addCredit" style="margin-right:10px">Add</button>
              </div>
              <table class="table">
                <thead>
                  <tr>
                    <th v-if="!isReadOnly">  </th>
                    <th>Account Name</th>
                    <th class="tableamount">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(credit, index) in credits" :key="credit.accountId" style="line-height:1; margin:0">
                    <td v-if="!isReadOnly"><icon icon="trash" v-on:click="deleteCredit(index)" style="margin-top:5px !important;"/></td>
                    <b-form-select
                      v-model="credit.accountId"
                      :options="accountsNV"
                      class="mb-3"
                      size="sm"
                      style="margin-top:10px"
                      :disabled="isReadOnly"
                    />
                    <td><input type="number" class="inputAmount" v-model="credit.amount" :disabled="isReadOnly"/></td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <footer class="modal-footer">
            <span v-if="isReadOnly">
              <button type="button" class="btn-green" @click="close" aria-label="Close">Close</button>
              <button type="button" class="btn-green" @click="change" aria-label="Change">Change</button>         
            </span>
            <span v-else>
              <button type="button" class="btn-green" @click="cancel" aria-label="Cancel">Cancel</button>
              <button type="button" class="btn-green" @click="save" aria-label="Save">Save</button>
            </span>
          </footer>
        </div>
      </div>
    </transition>
  </span>
</template>
<style>

.modaldetailjournal {
background: #ffffff;
box-shadow: 2px 2px 20px 1px;
overflow-x: auto;
display: flex;
height: flex;
flex-direction: column;
width: 800px !important;
max-height: 80%;
}

.dcrow {
  width: 800px;
}

.column {
  float: left;
  padding-left: 1%;
  width: 49%;
}

.b-form-select {
  padding-top: 4px;
}

@media screen and (max-width: 800px) {
  .modaldetail {
    width: 100% !important;
  }
  .column {
    width: 100%;
  }
  .dcrow {
    width: 100%;
  }
}
</style>