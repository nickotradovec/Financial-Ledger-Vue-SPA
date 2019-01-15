<script>
import modal from "./account-detail";

export default {
  components: {
    modal: modal
  },

  computed: {
    totalPages: function() {
      return accountList.length;
    }
  },

  data() {
    let account = {
      accountId: 0,
      accountName: "",
      commence_Formatted: "",
      cease_Formatted: ""
    };

    return {
      accountlist: []
    };
  },

  methods: {
    async loadPage() {
      try {
        let response = await this.$http.get(`/api/AccountList/AccountsList`);
        console.log(response.data);
        this.accountlist = response.data;
      } catch (err) {
        window.alert(err);
        console.log(err);
      }
    },
    addAccount() {
      this.$refs.modal.setMode(3);
    },
    viewAccount(id) {
      if (id == null || id <= 0) {
        alert("ID not set!");
      } else {
        this.$refs.modal.accountId = id;
        this.$refs.modal.setMode(1);
      }
    },

    closeModal() {
      // Handled my modal
      //this.$refs.modal.setMode(0); 
    },
    // Easiest and most robust to force a refresh of the whole object instead of emitting updates and adds.
    async closeModalWithRefresh() {
      //this.$refs.modal.setMode(0);
      console.log("refresh page on closemodalwithrefresh")
      await this.loadPage();
    }
  },

  async created() {
    this.loadPage();
  }
};
</script>

<template>
  <div>
    <h1>Accounts Management</h1>

    <button type="button" class="btn" @click="addAccount">Add Account</button>

    <modal @closeModal="closeModal" @closeModalWithRefresh="closeModalWithRefresh" ref="modal"></modal>

    <table v-if="accountlist" class="table">
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
          <td><a @click="viewAccount(account.accountId)">{{ account.accountName }}</a></td>
          <td>{{ account.commence_Formatted }}</td>
          <td>{{ account.cease_Formatted }}</td>
        </tr>
      </tbody>
    </table>

    <p v-else>
      <em>Loading...</em>
    </p>
  </div>
</template>