<script>
import formatting from "../libraries/formatting";

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
      var i;
      for (i = 0; i < this.accounts.length; i+=1) {
        actsNV.push({value: this.accounts[i].accountId, text: this.accounts[i].accountName})
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
      this.reset();
      this.mode = 0;
    },

    change() {
      console.log("change event raised.");
      this.mode = 2;
    },

    modalAddJournal() {
      this.mode = 3;
    },

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

        var i;
        for (i = 0; i < response.data.details.length; i++) {
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
        this.accounts = response.data
      } catch (err) {
        window.alert(err);
        console.log(err);
      }
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
          class="modaldetail"
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

              <b-form-select v-model="deletetest" :options="accountsNV" class="mb-3" />

            <div class="inputElement">
              <span v-if="journalId>=0">
                <label for="journalId">Journal Id</label>
                <input
                  type="number"
                  v-model="journalId"
                  placeholder="Journal Id"
                  id="journalId"
                  disabled="true"
                >
              </span>
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
              <h4>Debits</h4>
              <table class="table">
                <thead>
                  <tr>
                    <th>Id</th>
                    <th>Account Name</th>
                    <th style="text-align:right">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="debit in debits" :key="debit.accountId">
                    <td>{{debit.accountId}}</td>
                    <td>{{debit.accountName}}</td>
                    <!--<td>
                    <b-dropdown id="acclist" text="Select Account" class="m-md-2">
                      <b-dropdown-item>Test1</b-dropdown-item>
                      <b-dropdown-item>Test2</b-dropdown-item>
                    </b-dropdown>
                    </td>-->
                    <td style="text-align:right">{{debit.amount}}</td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="column" id="credits">
              <h4>Credits</h4>
              <table class="table">
                <thead>
                  <tr>
                    <!--<th>Id</th>-->
                    <th>Account Name</th>
                    <th style="text-align:right">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="credit in credits" :key="credit.accountId">
                    <!--<td>{{credit.accountId}}</td>
                    <td>{{credit.accountName}}</td>-->
                    <b-form-select v-bind="credit.accountId" :options="accountsNV" class="mb-3" size="sm"/>
                    <td>
                      <input
                        type="number"
                        v-bind="credit.amount"
                        :disabled="isReadOnly"
                        id="amount"
                      >
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <footer class="modal-footer">
            <span v-if="isReadOnly">
              <button type="button" class="btn-green" @click="change" aria-label="Change">Change</button>
              <button type="button" class="btn-green" @click="close" aria-label="Close">Close</button>
            </span>
            <span v-else>
              <button type="button" class="btn-green" @click="save" aria-label="Save">Save</button>
              <button type="button" class="btn-green" @click="close" aria-label="Change">Cancel</button>
            </span>
          </footer>
        </div>
      </div>
    </transition>
  </span>
</template>
<style>
.modal-backdrop {
  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background-color: rgba(0, 0, 0, 0.3);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modaldetail {
  background: #ffffff;
  box-shadow: 2px 2px 20px 1px;
  overflow-x: auto;
  display: flex;
  width: 800px;
  height: flex;
  flex-direction: column;
}

.modal-header,
.modal-footer {
  padding: 15px;
  display: flex;
}

.modal-header {
  border-bottom: 1px solid #eeeeee;
  color: #4aae9b;
  justify-content: space-between;
}

.modal-footer {
  border-top: 1px solid #eeeeee;
  justify-content: flex-end;
}

.modal-body {
  position: relative;
  padding: 20px 10px;
}

.btn-green {
  color: white;
  background: #4aae9b;
  border: 1px solid #4aae9b;
  border-radius: 2px;
}

input {
  float: right;
  width: 50%;
  text-align: right;
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

</style>