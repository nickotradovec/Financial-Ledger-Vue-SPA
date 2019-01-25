<script>
import formatting from "../libraries/formatting";
//TODO: IMPLEMENT PROPER ENUMERATION

export default {
  name: "modal",

  mixins: [formatting],

  computed: {
    modalVisible: function() {
      return this.mode > 0;
    },
    isReadOnly: function() {
      return this.mode <= 1;
    }
  },

  data() {
    return {
      mode: -1,
      accountId: -1,
      accountName: null,
      commence: null,
      cease: null,
      initialBalance: 0,
      validationErrors: []
    };
  },

  methods: {
    // Click events
    close() { 
      this.reset(); 
      this.mode = 0; },
    change() { 
      this.mode = 2; },
    cancel() { 
      if ( this.accountId >= 0 ) { this.getAccount(this.accountId, false); }
      else { this.reset(); } },
    modalAddAccount() { 
      this.commence = new Date(); 
      this.mode = 3; },

    save() {
      console.log(`Save event raised from mode: ${this.mode.toString()}`);
      //if (!this.validate()) { return; }

      if (this.accountId < 0) {
        console.log("Add new account");
        this.saveAccount(`/api/AccountManagement/AddNewAccount`);
      } else {
        console.log(`Save changes to account ${this.accountId}`);
        this.saveAccount(`/api/AccountManagement/UpdateAccount`);
      }
    },

    // Internal events
    reset() {
      console.log("reset called");
      this.mode = 0;
      this.accountId = -1;
      this.accountName = null;
      this.commence = null;
      this.cease = null;
      this.initialBalance = 0;
      this.validationErrors = [];
    },

    async getAccount(id, editmode) {
      if (id < 0) { console.log("ID not specified for getting account."); return; }
      this.accountId = id;

      try {
        console.log("Try retrive data for account: " & this.accountId);
        let response = await this.$http.get( `/api/AccountManagement/GetAccountDetails?id=${this.accountId}` );
        this.accountId = response.data.accountId;
        this.accountName = response.data.accountName;
        this.commence = this.dataDate(response.data.commence);
        this.cease = this.dataDate(response.data.cease);
        this.initialBalance = response.data.initialBalance;
       
        if (editmode) { this.mode = 2; } 
        else { this.mode = 1; }

      } catch (err) {
        window.alert("Unable to view the account at this time");
        this.reset();
        console.log(err);
      }
    },

    async saveAccount(api) {
      var comp = this;
      let dta = {
        accountId: this.accountId,
        accountName: this.accountName,
        commence: this.commence,
        cease: this.cease,
        initialBalance: this.initialBalance
      };
      console.log(dta);
      try {
        let response = await comp.$http.post(api, dta);
        comp.reset();
        comp.$emit("closeModalWithRefresh");
      } catch (err) {
        console.log(err);
        alert("unable to save account at this time.");
      }
    },

    validate() {
      this.validationErrors = [];
      var validate = true;
      if (!this.accountName || this.accountName.length <= 0) {
        this.validationErrors.push({ errorText: "Please specify a name" });
        validate = false;
      }
      console.log(this.commence);
      if (!this.commence || !this.validateDate(this.commence)) {
        this.validationErrors.push({ errorText: "Please specify a commence date." });
        validate = false;

      } else if (this.isFuture(this.commence)) {
        this.validationErrors.push({ errorText: "Commence may not be future dated." });
        validate = false;
      }

      return false; //validate;
    }
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
            <h3 v-if="mode==3">Add New Account</h3>
            <h3 v-else>Account Details</h3>
          </header>

          <section class="modal-body" id="modalDescription">
            <div v-if="validationErrors.length>0" class="validationErrors" id="validationErrors">
              <!--<p>Errors present. Please review the following:</p>-->
              <ul v-for="error in validationErrors" :key="error" style="margin-left:0;padding-left:0">
                <li style="list-style: none; padding-left:0px; color: maroon; line-height:1">{{error.errorText}}</li>
              </ul>
            </div>

            <div class="inputElement">
              <span v-if="accountId>=0">
              <label for="accountId">Account Id</label>
              <input
                type="number"
                v-model="accountId"
                placeholder="Account Id"
                id="accountId"
                disabled="true"
              >
              </span>
            </div>

            <div class="inputElement">
              <label for="accountName">Account Name</label>
              <input
                type="string"
                v-model="accountName"
                :disabled="isReadOnly"
                placeholder="Account Name"
                id="accountName"
              >
            </div>

            <div class="inputElement">
              <label for="commence">Commence Date</label>
              <input
                type="date"
                @input="dataDate(commence)"
                v-model="commence"
                :disabled="isReadOnly"
                id="commence"
              >
            </div>

            <div class="inputElement">
              <label for="cease">Cease Date</label>
              <input
                type="date"
                @input="dataDate(cease)"
                v-model="cease"
                :disabled="isReadOnly"
                placeholder="Cease Date"
                id="cease"
              >
            </div>

            <div class="inputElement">
              <label for="initialBalance">Starting Balance</label>
              <input
                type="number"
                v-model="initialBalance"
                :disabled="isReadOnly"
                id="initialBalance"
              >
            </div>
          </section>

          <footer class="modal-footer">
            <span v-if="isReadOnly">
              <button type="button" class="btn-green" @click="change" aria-label="Change">Change</button>              
              <button type="button" class="btn-green" @click="close" aria-label="Close">Close</button>
            </span>
            <span v-else>
               <button type="button" class="btn-green" @click="save" aria-label="Save">Save</button>               
              <button type="button" class="btn-green" @click="cancel" aria-label="Cancel">Cancel</button>       
            </span>
          </footer>
        </div>
      </div>
    </transition>
  </span>
</template>
<style>

.modaldetail {
  background: #ffffff;
  box-shadow: 2px 2px 20px 1px;
  overflow-x: auto;
  display: flex;
  height: flex;
  flex-direction: column;
  width: 400px !important;
}

</style>