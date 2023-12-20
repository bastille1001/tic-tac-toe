import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

const App = () => {
  const [player, setPlayer] = useState('X');
  const [board, setBoard] = useState([]);
  const [token, setToken] = useState('');
  const [message, setMessage] = useState('');
  const apiUrl = process.env.REACT_APP_BASE_URL;

  const createGame = async () => {
    try {
      const response = await axios.post(`${apiUrl}/api/Game`, {
        startingPlayer: player,
      });

      const { token, board } = response.data.data;
      setToken(token);
      setBoard(board);
      setMessage('');
    } catch (error) {
      console.error('Error creating game:', error);
    }
  };

  const makeMove = async (rowIndex, colIndex) => {
    try {
      if (!token) {
        return;
      }

      const moveResponse = await axios.post(`${apiUrl}/api/Game/makeMove`, {
        token,
        row: rowIndex,
        column: colIndex,
      });

      const { data } = moveResponse.data;

      const gameResponse = await axios.get(`${apiUrl}/api/Game?token=${token}`);

      const { board } = gameResponse.data.data;

      setBoard(board);

      if (data.includes('wins') || data.includes('draw')) {
        setToken('');
        setMessage(data);
      } else {
        setMessage('');
      }
    } catch (error) {
      console.error('Error making move:', error);
    }
  };

  return (
    <div className="app">
      <h1>Tic Tac Toe</h1>
      <div>
        Choose Player:{' '}
        <select onChange={(e) => setPlayer(e.target.value)}>
          <option value="X">X</option>
          <option value="O">O</option>
        </select>
      </div>
      <div>
        <button onClick={createGame}>Start New Game</button>
      </div>
      <div>
        <h2>Board</h2>
        {Array.isArray(board) && board.length > 0 ? (
          <div className="board">
            {board.map((row, rowIndex) => (
              <div key={rowIndex} className="row">
                {row.map((cell, colIndex) => (
                  <div
                    key={colIndex}
                    className="cell"
                    onClick={() => (token ? makeMove(rowIndex, colIndex) : null)}
                  >
                    {cell}
                  </div>
                ))}
              </div>
            ))}
          </div>
        ) : (
          <div>Loading...</div>
        )}
      </div>
      {message && <div className="message">{message}</div>}
    </div>
  );
};

export default App;
