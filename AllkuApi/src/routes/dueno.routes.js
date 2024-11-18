const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// Get all dueños
router.get('/', async (req, res) => {
  try {
    const duenosSnapshot = await db.collection('duenos').get();
    const duenos = [];
    duenosSnapshot.forEach(doc => {
      duenos.push({ id: doc.id, ...doc.data() });
    });
    res.json(duenos);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Get dueño by cedula
router.get('/:cedula', async (req, res) => {
  try {
    const duenoDoc = await db.collection('duenos').doc(req.params.cedula).get();
    if (!duenoDoc.exists) {
      return res.status(404).json({ message: 'Dueño no encontrado' });
    }
    res.json({ id: duenoDoc.id, ...duenoDoc.data() });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Create dueño
router.post('/', async (req, res) => {
  try {
    const { cedula_dueno, ...duenoData } = req.body;
    await db.collection('duenos').doc(cedula_dueno).set(duenoData);
    res.status(201).json({ id: cedula_dueno, ...duenoData });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Update dueño
router.put('/:cedula', async (req, res) => {
  try {
    const duenoRef = db.collection('duenos').doc(req.params.cedula);
    const dueno = await duenoRef.get();
    if (!dueno.exists) {
      return res.status(404).json({ message: 'Dueño no encontrado' });
    }
    await duenoRef.update(req.body);
    res.json({ message: 'Dueño actualizado exitosamente' });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Delete dueño
router.delete('/:cedula', async (req, res) => {
  try {
    const duenoRef = db.collection('duenos').doc(req.params.cedula);
    const dueno = await duenoRef.get();
    if (!dueno.exists) {
      return res.status(404).json({ message: 'Dueño no encontrado' });
    }
    await duenoRef.delete();
    res.json({ message: 'Dueño eliminado exitosamente' });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

module.exports = router;