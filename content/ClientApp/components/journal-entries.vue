<script>
import journalmodal from "./journal-detail";
import formatting from "../libraries/formatting";

export default {
  mixins: [formatting],

  components: {
    modal: journalmodal
  },

  data() {
    return {
      journallist: []
    };
  },

  methods: {
    async loadPage() {
      try {
        let response = await this.$http.get(`/api/JournalList/JournalEntries`);
        console.log(response.data);
        this.journallist = response.data;
        var par = this;
        this.journallist.forEach(function(jnl) {
          jnl.journalDate_Formatted = par.formatDate(jnl.journalDate);
        });
      } catch (err) {
        window.alert(err);
        console.log(err);
      }
    },

    addJournal() {
      this.$refs.modal.modalAddJournal();
    },

    viewJournal(id) {
      console.log(`Call viewAccount for account ${id}`);
      if (id == null || id <= 0) {
        alert("ID not set!");
      } else {
        this.$refs.modal.getJournal(id, false);
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
    <span v-if="journallist">
      <div class="header">
        <h1>Journal Entries</h1>
        <button type="button" class="btn" @click="addJournal">Add Journal Entry</button>
      <modal @closeModal="closeModal" @closeModalWithRefresh="closeModalWithRefresh" ref="modal"></modal>
      </div>     
      <table class="table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Reference Name</th>
            <th>Date</th>
            <th class="tableAmount">Amount</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="journal in journallist" :key="journal.journalId">
            <td>{{ journal.journalId }}</td>
            <td><a class="internalLink" @click="viewJournal(journal.journalId)">{{ journal.journalName}}</a></td>
            <td>{{ journal.journalDate_Formatted }}</td>
            <td class="tableAmount">{{ journal.journalAmount }}</td>
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
