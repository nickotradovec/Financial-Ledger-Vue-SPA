<script>
import accountListVue from "./account-list.vue";
//TODO: IMPLEMENT PROPER ENUMERATION
var modeEnum = Object.freeze({
  none: 0,
  view: 1,
  change: 2,
  add: 3,
  saveNew: 4,
  saveChanges: 5
});

import moment from "../libraries/moment.js";

export default {
  name: "modal",

  computed: {
    modalVisible: function() {
      return this.mode > 0;
    },
    isReadOnly: function() {
      if (this.mode === 2 || this.mode === 3) {
        return false; // can only change in change or add mode.
      }
      return true;
    }
  },

  data() {
    return {
      mode: -1,
      accountId: -1,
      accountName: "",
      commence: new Date(1900, 1, 1),
      cease: Date(9999, 12, 31),
      //commence_Formatted: "",
      //cease_Formatted: "",
      initialBalance: 0
    };
  },

  methods: {
    // Click events
    close() {
      console.log("Close event raised.");
      this.setMode(0);
    },

    change() {
      console.log("change event raised.");
      this.setMode(2);
    },

    save() {
      console.log(`Save event raised from mode: ${this.mode.toString()}`);
      switch (this.mode) {
        case 3:
          this.setMode(4); // Add -> SaveNew
          break;
        case 2:
          this.setMode(5); // Change -> SaveChanges
          break;
        default:
          alert(`Unexpected operation for mode: ${this.mode.toString()}`);
      }
    },

    // Internal events
    reset() {
      console.log("reset called");
      this.mode = 0;
      this.accountId = -1;
      this.accountName = "";
      this.commence = new Date(1900, 1, 1);
      this.commence = new Date(9999, 12, 31);
      this.initialBalance = 0;
    },

    async getAccount() {
      console.log("load accounts");
      if (this.accountId <= 0) {
        console.log("Account not set!");
      }
      try {
        console.log("Try retrive data for account: " & this.accountId);
        let response = await this.$http.get(
          `/api/Account/GetAccountDetails?id=${this.accountId}`
        );
        this.accountId = response.data.accountId;
        this.accountName = response.data.accountName;
        this.commence = response.data.commence;
        this.cease = response.data.cease;
        this.initialBalance = response.data.initialAccountBalance;
      } catch (err) {
        window.alert(err);
        console.log(err);
      }
    },

    saveNewAccount() {
      try {
        let dta = {
          accountId: this.accountId,
          accountName: this.accountName,
          commence: this.commence,
          cease: this.cease,
          initialBalance: this.initialAccountBalance
        };
        let response = this.$http.post(`/api/Account/AddNewAccount`, dta)
          .then;
        this.$emit("closeModalWithRefresh");
        this.reset();
      } catch (err) {
        console.log(err);
      }
    },

    saveAccountChanges() {
      console.log("attempting to call UpdateAccount");
      try {
        let dta = {
          accountId: this.accountId,
          accountName: this.accountName,
          commence: this.commence,
          cease: this.cease,
          initialBalance: this.initialAccountBalance
        };
        let response = this.$http.post(`/api/Account/UpdateAccount`, dta);
        this.$emit("closeModalWithRefresh");
        this.reset();
      } catch (err) {
        console.log(err);
      }
    },

    setMode(newMode) {
      console.log(`mode being changed from ${this.mode} to ${newMode}`);
      this.mode = newMode;
      switch (this.mode) {
        case 0: // None. Modal should not be open and we should have default data.
          this.reset();
          this.$emit("closeModal");
          break; // clear account
        case 1: // View
          this.getAccount();
          break;
        case 2: // Change. Prevent reload here if already loaded if desired.
          this.getAccount();
          break;
        case 3: // Add
          break;
        case 4: // SaveNew
          this.saveNewAccount();          
          break;
        case 5: // SaveChanges
          this.saveAccountChanges();
          break;
        default:
          console.log("Unexpected mode called!");
      }
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
            <span v-if="mode==3">Add New Account</span>
            <span v-else>Account Details: {{accountName}}</span>
          </header>

          <section class="modal-body" id="modalDescription">
            <label for="accountId">Account Id</label>
            <input
              type="number"
              v-model="accountId"
              placeholder="Account Id"
              id="accountId"
              disabled="true"
            >
            <br>
            <label for="accountName">Account Name</label>
            <input
              type="string"
              v-model="accountName"
              :disabled="isReadOnly"
              placeholder="Account Name"
              id="accountName"
            >
            <br>

            <label for="commence">Commence Date</label>
            <input
              type="date"
              v-model="commence"
              :disabled="isReadOnly"
              placeholder="Commence Date"
              id="commence"
            >
            <br>

            <label for="cease">Cease Date</label>
            <input
              type="date"
              v-model="cease"
              :disabled="isReadOnly"
              placeholder="Cease Date"
              id="cease"
            >
            <br>

            <label for="initialBalance">Starting Balance</label>
            <input
              type="number"
              v-model="initialBalance"
              placeholder="Cease Date"
              :disabled="isReadOnly"
              id="initialBalance"
            >
            <br>
          </section>

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
  width: 400px;
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

.btn-close {
  border: none;
  font-size: 20px;
  padding: 20px;
  cursor: pointer;
  font-weight: bold;
  color: #4aae9b;
  background: transparent;
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
</style>