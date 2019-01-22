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
    <h1>Journal Entries</h1>

    <span v-if="journallist">
      <div class="headerbuttons">
        <button type="button" class="btn" @click="addJournal">Add Journal Entry</button>
      </div>

      <modal @closeModal="closeModal" @closeModalWithRefresh="closeModalWithRefresh" ref="modal"></modal>
      <table class="table">
        <thead>
          <tr>
            <!--<th/>-->
            <th>ID</th>
            <th>Reference Name</th>
            <th>Date</th>
            <th>Amount</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="journal in journallist" :key="journal.journalId">
            <!--<td><font-awesome-icon icon="edit"/></td>-->
            <td>{{ journal.journalId }}</td>
            <td>
              <a
                class="journalLink"
                @click="viewJournal(journal.journalId)"
              >{{ journal.journalName}}</a>
            </td>
            <td>{{ journal.journalDate_Formatted }}</td>
            <td>{{ journal.journalAmount }}</td>
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
.btn {
  color: white;
  background: #4aae9b;
  border: 1px solid #4aae9b;
  border-radius: 2px;
}

.headerbuttons {
  padding-top: 5px;
  padding-bottom: 10px;
}

a.journalLink {
  color: #4aae9b !important;
}
</style>
