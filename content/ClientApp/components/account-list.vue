<script>
import modal from "./account-detail";
import formatting from "../libraries/formatting";

export default {
  mixins: [formatting],

  components: {
    modal: modal
  },

  data() {
    return {
      accountlist: []
    };
  },

  methods: {
    async loadPage() {
      try {
        let response = await this.$http.get(`/api/AccountList/AccountsList`);
        //console.log(response.data);
        this.accountlist = response.data;
        var par = this;
        this.accountlist.forEach(function(act) {
          (act.commence_Formatted = function() {
            return par.formatDate(act.commence);
          }),
            (act.cease_Formatted = function() {
              return par.formatDate(act.cease);
            });
          act.active = function() {
            return par.isActive(act.commence, act.cease);
          };
        });
        console.log(this.accountlist);
      } catch (err) {
        window.alert(err);
        console.log(err);
      }
    },

    addAccount() {
      this.$refs.modal.modalAddAccount();
    },

    viewAccount(id) {
      console.log(`Call viewAccount for account ${id}`);
      if (id == null || id <= 0) {
        alert("ID not set!");
      } else {
        this.$refs.modal.getAccount(id, false);
      }
    },

    closeModal() {},
    async closeModalWithRefresh() {
      await this.loadPage();
    }
  },

  async created() {
    await this.loadPage();
  }
};
</script>

<template>
  <div>
    <span v-if="accountlist">
      <div class="header">
        <h1 class="ttl">Accounts Management</h1>
        <button type="button" class="btn" @click="addAccount">Add Account</button>
      </div>

      <modal @closeModal="closeModal" @closeModalWithRefresh="closeModalWithRefresh" ref="modal"></modal>

      <table class="table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Commenced</th>
            <th>Ceased</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="account in accountlist" :key="account.accountId">
            <td>{{ account.accountId }}</td>
            <td> <a class="internalLink" @click="viewAccount(account.accountId)" >{{ account.accountName }}</a></td>
            <td>{{ account.commence_Formatted() }}</td>
            <td>{{ account.cease_Formatted() }}</td>
          </tr>
        </tbody>
      </table>
    </span>
    <p v-else>
      <em>Loading...</em>
    </p>
  </div>
</template>

<style>

</style>
