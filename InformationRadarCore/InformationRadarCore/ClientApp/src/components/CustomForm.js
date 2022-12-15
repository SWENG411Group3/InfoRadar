import React from 'react';

function CustomForm() {
  const [tableData, setTableData] = React.useState([]);

  const handleSubmit = (event) => {
    event.preventDefault();

    const formData = new FormData(event.target);
    const data = {};
    formData.forEach((value, key) => {
      data[key] = value;
    });

    setTableData([...tableData, data]);
  }

  return (
    <>
      <form onSubmit={handleSubmit}>
        <h2>LightHouse Custom Form:</h2>
        <h2>Unsized Types</h2>
        <select name="unsizedType">
          <option value="Integer Value">Integer Value</option>
          <option value="Long">Long</option>
          <option value="Small">Small</option>
          <option value="Real">Real</option>
          <option value="Text">Text</option>
          <option value="Date">Date</option>
        </select>
  
        <h2>Duration</h2>
        <label>Days: <input type="number" name="days" /></label><br />
        <label>Hours: <input type="number" name="hours" /></label><br />
        <label>Minutes: <input type="number" name="minutes" /></label><br />
        <label>Seconds: <input type="number" name="seconds" /></label>
  
        <h2>Table Name</h2>
        <input type="text" name="tableName" />
  
        <h2>Frequency</h2>
        <h3>Duration</h3>
        <label>Days: <input type="number" name="freqDays" /></label><br />
        <label>Hours: <input type="number" name="freqHours" /></label><br />
        <label>Minutes: <input type="number" name="freqMinutes" /></label><br />
        <label>Seconds: <input type="number" name="freqSeconds" /></label>
  
        <h3>Messenger Frequency</h3>
        <label>Days: <input type="number" name="messengerFreqDays" /></label><br />
        <label>Hours: <input type="number" name="messengerFreqHours" /></label><br />
        <label>Minutes: <input type="number" name="messengerFreqMinutes" /></label><br />
        <label>Seconds: <input type="number" name="messengerFreqSeconds" /></label>
  
        <h2>Thumbnail</h2>
        <input type="file" name="thumbnail" />
  
        <h2>LightHouse Enabled</h2>
        <input type="checkbox" name="enabled" />
  
        <br /><br />
        <input type="submit" value="Submit" />
      </form>
      <table>
        <thead>
          <tr>
            <th>Unsized Type</th>
            <th>Duration</th>
            <th>Table Name</th>
            <th>Frequency</th>
            <th>Messenger Frequency</th>
            <th>Thumbnail</th>
            <th>LightHouse Enabled</th>
          </tr>
        </thead>
      </table>
    </>
  );
}